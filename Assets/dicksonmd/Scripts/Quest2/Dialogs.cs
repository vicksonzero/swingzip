



using System;
using System.Collections.Generic;
using System.Linq;
public static class Dialogs
{
    // greetings from https://www.inklyo.com/18-ways-to-say-hello-in-english/
    public static string[] greetingsList = new string[]{
        "Hello!",
        "Greetings.",
        "Good day.",
        "It’s nice to meet you.",
        "It’s a pleasure to meet you.",
        "Hi!",
        "Morning!",
        "How are things with you?",
        "What’s new?",
        "It’s good to see you.",
        "G’day!",
        "Howdy!",
        "Hey there.",
        "What’s up?",
        "How’s it going?",
        "What’s happening",
        "Yo!",
    };

    private static Random random = new Random();
    public static string GetRandomGreetings()
    {
        int randomNumber = random.Next(0, greetingsList.Length);
        var greetings = greetingsList[randomNumber];
        return greetings;
    }
}