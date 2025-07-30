using System;

namespace TK2Bot.TK2RichPresenceTest
{
    public class DiscordHandler
    {
        private static DateTime BaseUtcTime { get; } = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime CurrentUtcTime => BaseUtcTime.AddSeconds(DateTime.UtcNow.Second);
        
        private readonly Discord.Discord m_discord;
        private readonly Discord.ActivityManager m_activityManager;
        private readonly Discord.UserManager m_userManager;
        private Discord.User CurrentUser => m_userManager.GetCurrentUser();

        private Discord.Activity m_currentActivity;
        
        private bool m_needActivityUpdate = false;
        
        private const Discord.ActivityPartyPrivacy PARTY_PRIVACY_DEBUG = Discord.ActivityPartyPrivacy.Private;

        public DiscordHandler(long _botClientId)
        {
            // Create the official DiscordGameSDK handler
            m_discord = new Discord.Discord(_botClientId, (ulong)Discord.CreateFlags.Default);
            m_discord.SetLogHook(Discord.LogLevel.Debug, (_level, _message) =>
            {
                Console.WriteLine($"Log[{_level}] {_message}");
            });

            // We are caching the some stuff for simpler future usage
            m_activityManager = m_discord.GetActivityManager();
            m_userManager = m_discord.GetUserManager();
        }

        ~DiscordHandler()
        {
            // We need to dispose of the DiscordGameSDK handler before deleting our custom handler
            m_discord.Dispose();
        }

        public void Initialize(uint _steamAppId = 0)
        {
            // We generate a Base Activity to be used 
            m_currentActivity = GenerateBaseActivity();
            
            // If we have a Steam App Id of the game to launch, Register it
            if (_steamAppId != 0)
                m_activityManager.RegisterSteam(_steamAppId);

            // Register the callbacks we are using for joining and else
            RegisterToCallbacks();
        }

        // Function we call regularly to update the Rich Presence if needed
        public bool RichPresenceUpdate()
        {
            try
            {
                // We let Discord and our DiscordGameSDK to its stuff
                m_discord.RunCallbacks();

                // If we have reacted to some stuff and need to update our activity ..
                if (m_needActivityUpdate)
                {
                    // .. we update our activity with the "new current" as we have changed it
                    m_activityManager.UpdateActivity(m_currentActivity, _result =>
                    {
                        if (_result != Discord.Result.Ok)
                            Console.WriteLine("ERROR: failed updating activity");
                    });
                    
                    // We set back the boolean to false to avoid Updating the Activity with the same activity each Update
                    m_needActivityUpdate = false;
                }

                return true;
            }
            catch
            {
                Console.WriteLine("ERROR: crash during Discord Callbacks");
            }

            return false;
        }

        // Generates the base Activity we will use during the Discord Handler's life
        private static Discord.Activity GenerateBaseActivity()
        {
            // This will be displayed when starting the game
            return new Discord.Activity
            {
                Name    = "The Karters 2 - AMAZING BETA", // Note: "Name" doesn't seem to change anything
                Details = "Currently testing Rich Presence",
                State   = "Just started",
                Assets  =
                {
                    LargeImage = "tk2_cover", // Name of the image set in the Discord Application (Bot)
                    LargeText  = "TK2 Game!",
                },
                Timestamps =
                {
                    Start = CurrentUtcTime.Millisecond
                },
                Type     = Discord.ActivityType.Playing,
                Instance = true, // Set to true here, but means we are currently in "an instance of match"
            };
        }

        // We register to the SDK's callbacks here
        private void RegisterToCallbacks()
        {
            // If the Discord User updates, we want to be aware of it
            m_userManager.OnCurrentUserUpdate += OnUserUpdateHandler;
            
            // Basic Activity situations
            m_activityManager.OnActivityInvite += OnInviteHandler; // Called when SOMEONE invites you (or a channel) to play
            m_activityManager.OnActivityJoin += OnJoinHandler; // Called when YOU are joining another player 
            m_activityManager.OnActivityJoinRequest += OnJoinRequestHandler; // Called when SOMEONE tries to join (and you need to answer)
            m_activityManager.OnActivitySpectate += OnSpectateHandler; // Called when YOU are joining as spectator another player
        }

        // Callback when Discord User changes - called at least one at the initialization
        private void OnUserUpdateHandler()
        {
            Discord.User user = CurrentUser;
            LogInfo($"Current UserName [{user.Username}] - Id [{user.Id}] - Discriminator [{user.Discriminator}]");

            // For debug. Here we are creating "Secrets" that will be used for joining players
            // For example, if we "Try to Join Another Player", we will receive THEIR secret key
            // If the player is in a specific lobby with code "548562", the JoinSecret could simply be "join548562" or anything else
            // The "JOINER" will then be able to try and join the lobby based on the info the "HOST" has sent them
            m_currentActivity.Secrets = new Discord.ActivitySecrets
            {
                Match    = $"fakeMatchSecret|{user.Username}|{user.Id}",
                Join     = $"fakeJoinSecret|{user.Username}|{user.Id}",
                Spectate = $"fakeSpectateSecret|{user.Username}|{user.Id}",
            };
            
            // For debug. Here we are creating the "Party" that is considered as "the party" in-game
            // For example, here we start as "CurrentSize 1". If someone joins, it should go up to 2. If someone leaves, it should go back to 1.
            // The Id of the Party can be anything; it's quite like a secret, but you can't simply "join using it". But Discord will regroup people using the same ID automatically in its display
            // Considering Privacy and MaxSize, those could be manually changed for example by the host of the game
            m_currentActivity.Party = new Discord.ActivityParty
            {
                Id = $"fakePartyId|{user.Username}|{user.Id}",
                Privacy = PARTY_PRIVACY_DEBUG,
                Size = new Discord.PartySize
                {
                    CurrentSize = 1,
                    MaxSize = 8
                }
            };
            
            // Since we have changed the Current Activity, we set the boolean to true so we will request Discord to update it
            m_needActivityUpdate = true;
        }

        // Callback when ANOTHER PLAYER is inviting us
        private void OnInviteHandler(Discord.ActivityActionType _type, ref Discord.User _user, ref Discord.Activity _otherPlayerActivity)
        {
            LogInfo($"OnInvite | Type[{_type}] UserName[{_user.Username}]\n" +
                    $"\t" +
                    $"WE RECEIVED A MESSAGE THROUGH DISCORD THAT INVITED US TO A GAME (we don't have any secret but we have the User which invited us) (can either be MP or in a Discord channel)");
            
            // Debug. We don't do anything, but here we could "accept the invitation" manually.
            // This could be made DIRECTLY IN THE GAME, since we can join using only "local code" without going on Discord (see `AcceptAnInviteFromPlayer()`)
            m_currentActivity.State      = "Received OnInvite";
            m_currentActivity.Timestamps = new Discord.ActivityTimestamps { Start = CurrentUtcTime.Millisecond };
            
            // Since we have changed the Current Activity, we set the boolean to true so we will request Discord to update it
            m_needActivityUpdate         = true;
        }

        // Callback when YOU are joining a player using a "secret" (a secret ID for the "join room")
        private void OnJoinHandler(string _secret)
        {
            LogInfo($"OnJoin | Secret[{_secret}]\n" +
                    $"\t" +
                    $"WE ARE NOW JOINING THE PLAYER USING THE SECRET THEY GAVE US (would probably be the same as \"Join player\")");

            // For debug. We are currently considering the secret as "fakeJoinSecret_username_userid"
            string[] splitSecret = _secret.Split('|');
            string username = splitSecret[1];
            string userid = splitSecret[2];
            
            // HERE we should be joining the game correctly
            // THEN WHEN we have correctly joined..
            
            // ..we can set the new info based on the current game we are in!
            m_currentActivity.State      = "Joined game!";
            m_currentActivity.Timestamps = new Discord.ActivityTimestamps { Start = CurrentUtcTime.Millisecond };
            
            // Here, we are setting the fakePartyId, and changing the current size to one.
            // If in a real game, this could change based on other player's joining or leaving, or the host changing settings
            m_currentActivity.Party = new Discord.ActivityParty
            {
                Id      = $"fakePartyId|{username}|{userid}",
                Privacy = PARTY_PRIVACY_DEBUG,
                Size = new Discord.PartySize
                {
                    CurrentSize = 2,
                    MaxSize     = 8
                }
            };
            
            // Since we have changed the Current Activity, we set the boolean to true so we will request Discord to update it
            m_needActivityUpdate         = true;
        }

        // Callback when ANOTHER PLAYER has requested to join your party
        private void OnJoinRequestHandler(ref Discord.User _user)
        {
            string requesterUsername = _user.Username;
            
            LogInfo($"OnJoinRequest | UserName[{requesterUsername}]");

            // For debug. In our case, we are always accepting the request (will actually answer with a MP acting as "invite")
            // In a REAL GAME, we will probably be able to have displayed a notification and answer "yes" or "no" through the game directly. Then the other player will do the same to join.
            m_activityManager.SendRequestReply(_user.Id, Discord.ActivityJoinRequestReply.Yes, _result =>
            {
                if (_result == Discord.Result.Ok)
                    LogInfo($"OnJoinRequest | ACCEPTED UserName[{requesterUsername}] TO JOIN");
                else
                    LogInfo($"OnJoinRequest | FAIL TO ACCEPT UserName[{requesterUsername}] TO JOIN");
            });

            // For debug purposes.
            m_currentActivity.State      = "Received OnJoinRequest";
            m_currentActivity.Timestamps = new Discord.ActivityTimestamps { Start = CurrentUtcTime.Millisecond };
            m_needActivityUpdate         = true;
        }

        // Callback when YOU are joining a player using a "secret" (a secret ID for the "spectate room")
        private void OnSpectateHandler(string _secret)
        {
            // Could not test yet, though it seems it acts exactly like "joining" but with other purpose and another secret (to avoid joining the room as a player)
            LogInfo($"OnSpectate | Secret[{_secret}]");
            
            // For debug purposes. BUT we are actually changing the "activity type" to "Watching" as we are now spectating and not playing anymore
            // On another note, in a "real game", when joining a "already playing game", the Type could also be changed.
            // Note: based on documentation, it seems more ActivityType exists ("custom" or "competing" for instance)
            m_currentActivity.State      = "Received OnSpectate";
            m_currentActivity.Type       = Discord.ActivityType.Watching;
            m_currentActivity.Timestamps = new Discord.ActivityTimestamps { Start = CurrentUtcTime.Millisecond };
            m_needActivityUpdate         = true;
        }

        // Assuming we know the ID of the player we want to invite - NOT TESTED
        private void SendAnInviteToPlayer(long _userId)
        {
            // For debug. We simply get the User information instead of only its UserId
            Discord.User invitedUser = new Discord.User();
            m_userManager.GetUser(_userId, (Discord.Result _result, ref Discord.User _outUser) => { invitedUser = _outUser; });
            string invitedUsername = "INVALID";
            if (invitedUser.Username != null)
            {
                invitedUsername = invitedUser.Username;
            }

            // We send an invite instantly as MP to the user to join their game. We can join as we can spectate.
            m_activityManager.SendInvite(_userId, Discord.ActivityActionType.Join, "Come play!", _result =>
            {
                if (_result == Discord.Result.Ok)
                    LogInfo($"SendInvite | SUCCESS UserName[{invitedUsername}] INVITED");
                else
                    LogInfo($"SendInvite | FAILED TO INVITE UserName[{invitedUsername}]");
            });
        }
        
        // Assuming we know the ID of the player that invited us - NOT TESTED
        private void AcceptAnInviteFromPlayer(long _userId)
        {
            // For debug. We simply get the User information instead of only its UserId
            Discord.User playerThatInvitedUs = new Discord.User();
            m_userManager.GetUser(_userId, (Discord.Result _result, ref Discord.User _outUser) => { playerThatInvitedUs = _outUser; });
            string invitedUsername = "INVALID";
            if (playerThatInvitedUs.Username != null)
            {
                invitedUsername = playerThatInvitedUs.Username;
            }

            // We accept an invite instantly from a User to be able to join their game
            m_activityManager.AcceptInvite(_userId, _result =>
            {
                if (_result == Discord.Result.Ok)
                    LogInfo($"AcceptInvite | SUCCESS ACCEPTED UserName[{invitedUsername}] INVITE");
                else
                    LogInfo($"AcceptInvite | FAILED TO ACCEPT UserName[{invitedUsername}] INVITE");
            });
        }

        // For debug purposes
        private void LogInfo(string _logLine)
        {
            Console.WriteLine($"[INFO] {CurrentUser.Username}:\n\t{_logLine}\n");
        }
    }
}
