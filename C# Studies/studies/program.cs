using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;

class Program
{
    // ========== Configuration Constants ==========
    private const string BOT_NAME = "Chain-19";
    private const int TYPING_DELAY_MS = 20;
    private const string GREETING_SOUND = "greeting.wav";

    // ========== Data Structures ==========
    private static readonly Random _random = new Random();
    private static string _currentTopic = "";
    private static string _currentSentiment = "neutral";
    private static readonly UserProfile _userProfile = new UserProfile();

    private class UserProfile
    {
        public string Name { get; set; } = "";
        public string FavoriteTopic { get; set; } = "";
        public List<string> DiscussedTopics { get; } = new List<string>();
        public Dictionary<string, string> PersonalInfo { get; } = new Dictionary<string, string>();
    }

    // ========== Response Collections ==========
    private static readonly Dictionary<string, string[]> _topicResponses = new Dictionary<string, string[]>
    {
        ["password"] = new[] {
            "Use long passphrases (like 'PurpleTiger$JumpsHigh') instead of simple passwords.",
            "Never reuse passwords across different accounts - a breach in one service could compromise all.",
            "Enable two-factor authentication wherever possible, even if you have strong passwords.",
            "Consider using a password manager - it's like a vault for all your passwords."
        },
        ["phishing"] = new[] {
            "Check sender email addresses carefully - scammers often use addresses that look similar to real ones.",
            "Hover over links before clicking to see the actual URL. If it looks suspicious, don't click!",
            "Be cautious of emails creating urgency ('Your account will be closed!') - common phishing tactic.",
            "Look for poor grammar and spelling - many phishing attempts originate from non-native speakers."
        },
        ["scam"] = new[] {
            "If an offer seems too good to be true, it probably is. Trust your instincts!",
            "Never share verification codes with anyone - legitimate companies will never ask for these.",
            "Be wary of urgent requests for money or information - scammers often create false emergencies."
        },
        ["privacy"] = new[] {
            "Regularly review app permissions on your devices - many apps request more access than they need.",
            "Use private browsing when you don't want history saved, but remember it doesn't make you anonymous.",
            "Consider using a VPN when on public WiFi to encrypt your internet traffic.",
            "Be mindful of what you post on social media - even 'private' accounts can be compromised."
        }
    };

    private static readonly Dictionary<string, string[]> _sentimentResponses = new Dictionary<string, string[]>
    {
        ["worried"] = new[] {
            "I understand this can feel overwhelming. Let's take it step by step.",
            "It's normal to feel concerned. There are simple steps to protect yourself.",
            "Don't worry - I'll help you through this."
        },
        ["frustrated"] = new[] {
            "I hear your frustration. Let's work through this together.",
            "Tech issues can be annoying. We'll figure this out.",
            "I understand this is frustrating. Let me help."
        },
        ["confused"] = new[] {
            "No worries! Let me explain this clearly.",
            "It's okay to feel unsure. I'll break this down for you.",
            "This can be tricky! Let me simplify it."
        },
        ["positive"] = new[] {
            "That's great! Let's build on that energy.",
            "Wonderful! Your enthusiasm helps with security.",
            "Fantastic! Your attitude will help you stay safe."
        },
        ["grateful"] = new[] {
            "You're welcome! Happy to help you stay safe.",
            "Thanks for your kind words!",
            "My pleasure! Ask me anything else."
        }
    };

    private static readonly Dictionary<string, string> _detailedExplanations = new Dictionary<string, string>
    {
        ["password"] = "🔐 Password security is fundamental. Strong passwords should be:\n" +
                      "- Long (12+ characters)\n- Unique for each account\n" +
                      "- Include letters, numbers, and symbols\n" +
                      "Password managers help generate and store these securely.",
        ["phishing"] = "🎣 Phishing is when attackers pretend to be trustworthy entities to:\n" +
                      "- Steal login credentials\n- Install malware\n- Access financial info\n" +
                      "Always verify sender identity before clicking links or downloading attachments.",
        ["scam"] = "🚨 Common online scams include:\n" +
                  "- Fake tech support\n- Romance scams\n- Investment frauds\n" +
                  "They typically create urgency or offer unrealistic rewards.",
        ["privacy"] = "🛡️ Online privacy involves controlling:\n" +
                     "- What personal information you share\n- Who can access it\n" +
                     "- How companies use your data\n" +
                     "Regularly check privacy settings on all accounts and devices."
    };

    private static readonly Dictionary<string, ConsoleColor> _sentimentColors = new Dictionary<string, ConsoleColor>
    {
        ["worried"] = ConsoleColor.DarkYellow,
        ["frustrated"] = ConsoleColor.DarkMagenta,
        ["confused"] = ConsoleColor.DarkCyan,
        ["positive"] = ConsoleColor.Green,
        ["grateful"] = ConsoleColor.Cyan,
        ["neutral"] = ConsoleColor.White
    };

    private static readonly string[] _unknownInputResponses = {
        "I'm not sure I understand. Could you try rephrasing?",
        "I specialize in cybersecurity topics. Try asking about passwords, phishing, or privacy.",
        "I didn't catch that. Maybe ask about password safety or online scams?",
        "How about we discuss cybersecurity? For example: 'How do I spot phishing emails?'"
    };

    // ========== Main Program Flow ==========
    static void Main()
    {
        try
        {
            InitializeConsole();
            PlayGreeting();
            DisplayHeader();
            GetUserName();
            StartChat();
        }
        catch (Exception ex)
        {
            HandleCriticalError(ex);
        }
    }

    // ========== Initialization Methods ==========
    private static void InitializeConsole()
    {
        Console.Title = $"{BOT_NAME} - Cybersecurity Assistant";
        Console.WindowWidth = Math.Min(100, Console.LargestWindowWidth);
        Console.WindowHeight = Math.Min(30, Console.LargestWindowHeight);
    }

    private static void DisplayHeader()
    {
        try
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
   _____ _           _     _         
  / ____| |         | |   (_)        
 | |    | |__   __ _| |__  _ _ __    
 | |    | '_ \ / _` | '_ \| | '_ \   
 | |____| | | | (_| | | | | | | | |  
  \_____|_| |_|\__,_|_| |_|_|_| |_|  
                                      
    Error Studio Labs Chain 19
--------------------------------");
            Console.ResetColor();
            Thread.Sleep(1000);
        }
        catch
        {
            // If header display fails, continue anyway
        }
    }

    private static void PlayGreeting()
    {
        try
        {
            using (var player = new SoundPlayer(GREETING_SOUND))
            {
                player.PlaySync();
            }
        }
        catch 
        { 
            // Continue without sound if there's an error
        }
    }

    private static void GetUserName()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        TypeEffect("📝 What's your name? ");
        Console.ResetColor();
        _userProfile.Name = Console.ReadLine()?.Trim() ?? "User";
    }

    // ========== Core Chat Functionality ==========
    private static void StartChat()
    {
        try
        {
            DisplayWelcomeMessage();
            ShowMainMenu();

            while (true)
            {
                try
                {
                    string userInput = GetUserInput();
                    if (ShouldExitChat(userInput)) break;

                    ProcessUserMessage(userInput);
                }
                catch (Exception ex)
                {
                    HandleChatError(ex);
                }
            }
        }
        catch (Exception ex)
        {
            HandleCriticalError(ex);
        }
    }

    private static void DisplayWelcomeMessage()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        TypeEffect($"\n[ChatBot]: Welcome {_userProfile.Name}! I'm your Cybersecurity Assistant.");
        Console.ResetColor();
    }

    private static string GetUserInput()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("\n🧑 You: ");
        Console.ResetColor();
        
        string input = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            TypeEffect("[ChatBot]: I didn't catch that. Could you repeat or rephrase?");
            Console.ResetColor();
        }
        return input;
    }

    private static bool ShouldExitChat(string input)
    {
        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            TypeEffect($"[ChatBot]: Stay safe online, {_userProfile.Name}! Goodbye!");
            Console.ResetColor();
            return true;
        }
        return false;
    }

    private static void ProcessUserMessage(string userInput)
    {
        DetectSentiment(userInput);
        string response = GenerateResponse(userInput);
        DisplayResponse(response);
    }

    // ========== Response Generation ==========
    private static string GenerateResponse(string userInput)
    {
        if (IsMemoryCommand(userInput))
            return HandleMemoryCommand(userInput);

        if (IsMenuRequest(userInput))
        {
            ShowMainMenu();
            return "";
        }

        if (IsPersonalInfo(userInput))
            StorePersonalInfo(userInput);

        return ProcessTopicBasedInput(userInput);
    }

    private static string ProcessTopicBasedInput(string userInput)
    {
        // Handle follow-ups for current topic
        if (!string.IsNullOrEmpty(_currentTopic))
        {
            if (IsFollowUpRequest(userInput))
                return GetFollowUpResponse();

            if (IsExplanationRequest(userInput))
                return GetDetailedResponse();
        }

        // Detect new topic
        string detectedTopic = DetectTopic(userInput);
        if (!string.IsNullOrEmpty(detectedTopic))
        {
            _currentTopic = detectedTopic;
            _userProfile.DiscussedTopics.Add(detectedTopic);
            return GetTopicResponse(isFollowUp: false);
        }

        // Default response for unknown input
        return GetUnknownInputResponse();
    }

    private static string GetFollowUpResponse()
    {
        string sentimentPrefix = GetSentimentResponse();
        return $"{sentimentPrefix} {GetTopicResponse(isFollowUp: true)}";
    }

    private static string GetDetailedResponse()
    {
        string sentimentPrefix = GetSentimentResponse();
        string explanation = _detailedExplanations.ContainsKey(_currentTopic) 
            ? _detailedExplanations[_currentTopic] 
            : "Cybersecurity protects systems and data from digital attacks.";

        return _currentSentiment switch
        {
            "worried" => $"{sentimentPrefix}\n\n{explanation}\n\nRemember, implement these steps gradually.",
            "frustrated" => $"{sentimentPrefix}\n\n{explanation}\n\nEach small step improves your security.",
            "confused" => $"{sentimentPrefix}\n\n{explanation}\n\nWould you like me to focus on any specific part?",
            _ => $"{sentimentPrefix}\n\n{explanation}"
        };
    }

    private static string GetTopicResponse(bool isFollowUp)
    {
        if (!_topicResponses.ContainsKey(_currentTopic))
            return "Let's focus on cybersecurity best practices...";

        string prefix = isFollowUp ? "Here's another tip" : "Great question";
        prefix = GetResponsePrefix(prefix, isFollowUp);

        if (!string.IsNullOrEmpty(_userProfile.FavoriteTopic) && _currentTopic == _userProfile.FavoriteTopic)
            prefix = $"Since you like {_currentTopic}, {prefix.ToLower()}";

        string[] responses = _topicResponses[_currentTopic];
        return $"{prefix} about {_currentTopic}: {responses[_random.Next(responses.Length)]}";
    }

    private static string GetResponsePrefix(string defaultPrefix, bool isFollowUp)
    {
        return _currentSentiment switch
        {
            "worried" => isFollowUp ? "Here's reassuring info" : "I understand your concerns",
            "frustrated" => isFollowUp ? "Let's try this approach" : "I hear your frustration",
            "confused" => isFollowUp ? "Let me explain differently" : "Good question",
            _ => defaultPrefix
        };
    }

    // ========== Helper Methods ==========
    private static void ShowMainMenu()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            TypeEffect("\n🔍 You can ask me about:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            TypeEffect("- Passwords (creating, securing, managing)");
            TypeEffect("- Phishing (recognition, prevention)");
            TypeEffect("- Scams (identification, protection)");
            TypeEffect("- Privacy (online protection, settings)");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            TypeEffect("Say 'menu' to see options or 'exit' to quit");
            Console.ResetColor();
        }
        catch
        {
            // If menu display fails, continue anyway
        }
    }

    private static void DetectSentiment(string input)
    {
        try
        {
            if (ContainsAny(input, "worried", "concerned", "scared", "anxious", "nervous", "afraid"))
                _currentSentiment = "worried";
            else if (ContainsAny(input, "frustrated", "angry", "annoyed", "upset", "mad"))
                _currentSentiment = "frustrated";
            else if (ContainsAny(input, "confused", "unsure", "don't know", "dont know", "help"))
                _currentSentiment = "confused";
            else if (ContainsAny(input, "excited", "happy", "great", "good", "awesome", "love"))
                _currentSentiment = "positive";
            else if (ContainsAny(input, "thank", "thanks", "appreciate", "helpful"))
                _currentSentiment = "grateful";
            else
                _currentSentiment = "neutral";
        }
        catch
        {
            _currentSentiment = "neutral";
        }
    }

    private static string GetSentimentResponse()
    {
        if (_currentSentiment == "neutral" || !_sentimentResponses.ContainsKey(_currentSentiment))
            return "";

        string[] responses = _sentimentResponses[_currentSentiment];
        return responses[_random.Next(responses.Length)];
    }

    private static bool IsMemoryCommand(string input) => 
        input.Contains("remember") || input.Contains("recall");

    private static bool IsMenuRequest(string input) => 
        input.Contains("menu") || input.Contains("help");

    private static bool IsPersonalInfo(string input) => 
        input.StartsWith("my ") || input.StartsWith("i ");

    private static bool IsFollowUpRequest(string input) => 
        ContainsAny(input, "more", "another", "else", "different");

    private static bool IsExplanationRequest(string input) => 
        ContainsAny(input, "explain", "what do you mean", "clarify", "confused");

    private static string DetectTopic(string input)
    {
        if (ContainsAny(input, "password", "passwords", "credential", "login"))
            return "password";
        if (ContainsAny(input, "phish", "phishing"))
            return "phishing";
        if (ContainsAny(input, "scam", "fraud", "hoax"))
            return "scam";
        if (ContainsAny(input, "privacy", "private", "data protection", "tracking"))
        {
            if (ContainsAny(input, "interested", "like", "love", "favorite"))
                _userProfile.FavoriteTopic = "privacy";
            return "privacy";
        }
        return "";
    }

    private static string HandleMemoryCommand(string input)
    {
        try
        {
            if (input.Contains("my name"))
                return $"I remember your name is {_userProfile.Name}!";

            if (input.Contains("favorite topic") || input.Contains("interested in"))
            {
                if (!string.IsNullOrEmpty(_userProfile.FavoriteTopic))
                    return $"I remember you're interested in {_userProfile.FavoriteTopic}. " + 
                           GetPersonalizedTip(_userProfile.FavoriteTopic);
                
                return "I don't recall you mentioning a favorite topic yet. What interests you?";
            }

            if (input.Contains("we talked") || input.Contains("discussed"))
            {
                if (_userProfile.DiscussedTopics.Count > 0)
                    return $"We've discussed: {string.Join(", ", _userProfile.DiscussedTopics)}. " +
                           "Want to revisit any?";
                
                return "We haven't discussed topics yet. What would you like to know?";
            }

            return "I can remember your name and favorite topics. Try asking 'what's my name?'";
        }
        catch
        {
            return "I'm having trouble recalling information right now.";
        }
    }

    private static void StorePersonalInfo(string input)
    {
        try
        {
            if (input.Contains("email") || input.Contains("address"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                TypeEffect("[ChatBot]: For security, I don't store sensitive info like emails.");
                Console.ResetColor();
            }
            else if (input.Contains("favorite") || input.Contains("interested"))
            {
                string topic = DetectTopic(input);
                if (!string.IsNullOrEmpty(topic))
                    _userProfile.FavoriteTopic = topic;
            }
        }
        catch
        {
            // Ignore storage errors
        }
    }

    private static string GetPersonalizedTip(string topic)
    {
        return topic switch
        {
            "password" => "🔐 Personalized tip: Change passwords every 3-6 months and never share them!",
            "phishing" => "🎣 Personalized tip: Bookmark important sites instead of clicking email links.",
            "scam" => "🚨 Personalized tip: Verify unexpected requests by contacting the company directly.",
            "privacy" => "🛡️ Personalized tip: Use private browsing for sensitive activities.",
            _ => "⭐ General tip: Update software regularly for security patches."
        };
    }

    private static string GetUnknownInputResponse() => 
        _unknownInputResponses[_random.Next(_unknownInputResponses.Length)];

    private static void DisplayResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response)) return;
        
        Console.ForegroundColor = _sentimentColors.ContainsKey(_currentSentiment) 
            ? _sentimentColors[_currentSentiment] 
            : ConsoleColor.White;
        
        TypeEffect($"[ChatBot]: {response}");
        Console.ResetColor();
    }

    private static bool ContainsAny(string input, params string[] keywords)
    {
        try
        {
            foreach (string keyword in keywords)
            {
                if (input.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private static void TypeEffect(string message)
    {
        try
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(TYPING_DELAY_MS);
            }
            Console.WriteLine();
        }
        catch
        {
            Console.WriteLine(message);
        }
    }

    // ========== Error Handling ==========
    private static void HandleChatError(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        TypeEffect($"[ChatBot]: Oops! Something went wrong. Let's try again.");
        Console.ResetColor();
        Console.Error.WriteLine($"Error: {ex.Message}");
    }

    private static void HandleCriticalError(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n⚠️  Critical error: {ex.Message}");
        Console.ResetColor();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}