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

        private Discord.Activity m_currentActivity;
        
        private bool m_needActivityUpdate = false;

        public DiscordHandler(long _botClientId)
        {
            m_discord = new Discord.Discord(_botClientId, (ulong)Discord.CreateFlags.Default);
            m_discord.SetLogHook(Discord.LogLevel.Debug, (_level, _message) =>
            {
                Console.WriteLine($"Log[{_level}] {_message}");
            });

            m_activityManager = m_discord.GetActivityManager();
            m_userManager = m_discord.GetUserManager();
        }

        ~DiscordHandler()
        {
            m_discord.Dispose();
        }

        public void Initialize()
        {
            Discord.ApplicationManager applicationManager = m_discord.GetApplicationManager();
            Console.WriteLine($"Current Locale: {applicationManager.GetCurrentLocale()}");
            Console.WriteLine($"Current Branch: {applicationManager.GetCurrentBranch()}");

            m_currentActivity = GenerateBaseActivity();

            RegisterToCallbacks();
        }

        public bool RichPresenceUpdate()
        {
            try
            {
                m_discord.RunCallbacks();

                if (m_needActivityUpdate)
                {
                    m_activityManager.UpdateActivity(m_currentActivity, _result =>
                    {
                        if (_result != Discord.Result.Ok)
                            Console.WriteLine("ERROR: failed updating activity");
                    });
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

        private static Discord.Activity GenerateBaseActivity()
        {
            return new Discord.Activity
            {
                Name    = "The Karters 2 - AMAZING BETA",
                Details = "Currently testing Rich Presence",
                State   = "Just started",
                Assets  =
                {
                    LargeImage = "tk2_cover",
                    LargeText  = "TK2 Game!",
                },
                Timestamps =
                {
                    Start = CurrentUtcTime.Millisecond
                },
                Type    = Discord.ActivityType.Playing,
                Secrets = new Discord.ActivitySecrets
                {
                    Join     = "invalidJoinSecret",
                    Spectate = "invalidSpectateSecret",
                    Match    = "invalidMatchSecret",
                },
                Party = new Discord.ActivityParty
                {
                    Id      = "fakePartyId",
                    Privacy = Discord.ActivityPartyPrivacy.Public,
                    Size    = new Discord.PartySize
                    {
                        CurrentSize = 3,
                        MaxSize     = 8
                    }
                },
            };
        }

        private void RegisterToCallbacks()
        {
            m_userManager.OnCurrentUserUpdate += OnUserUpdateHandler;
            
            m_activityManager.OnActivityInvite += OnInviteHandler;
            m_activityManager.OnActivityJoin += OnJoinHandler;
            m_activityManager.OnActivityJoinRequest += OnJoinRequestHandler;
            m_activityManager.OnActivitySpectate += OnSpectateHandler;
        }

        private void OnUserUpdateHandler()
        {
            Discord.User user = m_userManager.GetCurrentUser();
            Console.WriteLine($"Current UserName [{user.Username}] - Id [{user.Id}]");

            m_currentActivity.Secrets = new Discord.ActivitySecrets { Join = $"fakeJoinSecret_{user.Username}_{user.Id}", Spectate = $"fakeSpectateSecret_{user.Username}_{user.Id}", Match = $"fakeMatchSecret_{user.Username}_{user.Id}", };

            m_needActivityUpdate = true;
        }

        private void OnInviteHandler(Discord.ActivityActionType _type, ref Discord.User _user, ref Discord.Activity _activity2)
        {
            Console.WriteLine($"OnInvite {_type}, {_user} {_activity2}");
            
            m_currentActivity.State      = "Received OnInvite";
            m_currentActivity.Timestamps = new Discord.ActivityTimestamps { Start = CurrentUtcTime.Millisecond };
            m_needActivityUpdate         = true;
        }

        private void OnJoinHandler(string _secret)
        {
            Console.WriteLine($"OnJoin {_secret}");
            
            m_currentActivity.State      = "Received OnJoin";
            m_currentActivity.Timestamps = new Discord.ActivityTimestamps { Start = CurrentUtcTime.Millisecond };
            m_needActivityUpdate         = true;
        }

        private void OnJoinRequestHandler(ref Discord.User _user)
        {
            Console.WriteLine($"OnJoinRequest {_user}");
            
            m_currentActivity.State      = "Received OnJoinRequest";
            m_currentActivity.Timestamps = new Discord.ActivityTimestamps { Start = CurrentUtcTime.Millisecond };
            m_needActivityUpdate         = true;
        }

        private void OnSpectateHandler(string _secret)
        {
            Console.WriteLine($"OnSpectate {_secret}");
            
            m_currentActivity.State      = "Received OnSpectate";
            m_currentActivity.Timestamps = new Discord.ActivityTimestamps { Start = CurrentUtcTime.Millisecond };
            m_needActivityUpdate         = true;
        }
    }
}
