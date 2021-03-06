﻿namespace TextToSpeechTest
{
    using System.Collections.Generic;

    using AcapelaGroup.BabTTSNet;

    public class AcapelaEngineProvider : IEngineProvider
    {
        private const string AcaTtsPath = "C:\\Program Files (x86)\\Acapela Group\\Acapela TTS for Windows\\AcaTTS.dll";

        private readonly BabTTS engine = new BabTTS();

        static AcapelaEngineProvider()
        {
            Helpers.AcaTTSPath = AcaTtsPath;
            Helpers.Bundle(
                "#REVT30330NsH7oktT4RU6jZ8I8ZAIa58J45d7!EF7asfVjRAGAkvWVEXQjZv5yMt6vgdVbAU4%@AUcsdZ!4c6l5v4aUd7vgVKVws5toVKSUF7!4c6lBvZa4d6lJv6w8t4wAVQ9Ed5!4c6lJv6wgt6doVKQYs5w4d6lNv5cct4w@QVxAc6rgF4bYU5FMs5qsVY%@QVcEt5$@oWaUd7!UF7lhX7cMs5pst4w@8Qx8t6doVKGkFYaEU5bEVYr@QQtUF7!UF7l5eYvgt6doVKFMs5qsVY%@8RbYU6!4c645HYrUdZrAtL3kF7uVfYcQBInRpIaxQI8xoIfRBIeVQI4xbRnQs5j5AGGYU7qcAIbJAGEEFZj58Id58I4$eVvAs4LwbVvAs4j$tWtEXR%UuXBEt7mtd72IvY7UcQOgtKXYWVQkXW4QHQrtFVm48R6Md2xssUHcHIp4AR6YX6V8fKcEdZSMEIM48YTQG5qwUVU4VXnpWJyAW49xuJcsFVEEGSbZW5CUR5nQE6swu4fYtYHQXRWUFYdJeU5URXOIf6Rwc5HEoUCUc6n9tN8MoM%AGUM4HZT8HQ6EXVCIt73YUYSMdNvIcW5sWUAoGYDMu7awtSEYvKQEF68MXW8FV6FEHQ8tWUkAQ5T4XUeooKx4vSXMuR!wUXs@tNSIA6YUdXToW695uY6su5A8dKbwF4VABY$gXUVApMYosVdJoRPUHYbUcYaVc6Z8vZF4AY5@d7oMo5OUH7oEQ5wQcV$MQR6MtWrF9QEwsNxYV5fwdNSs8QfYUVsofJfEp2vFHSwUHVQsvMmUeKCIVXQIuJqMWZvtuSEUFVd@HIegVVAgFY9lt4CoGVCYGSn4FWPktNCsF6fEs5O4s4cJQVy4e7V4HWvUdRrsuJmFdNSEEReYGQaxWU2stS5QfJVktUahvJxUpSkAEZAIE7VQc2FsfVdVu2Dsf5A@vZY@VYGMcRmMv6OIeU8UdR8NuJdQs2EcV42Ad6EseJAQf2aEUQooQ74IURyQWZBMUU2EF7$4s4wsXWyoo5HwX4SsuKqYGRdUURBIuJokVXc@vQ8oHIDsHWeIuQD4GZqJuN@AH5ZYsWQAQQcEV7PMWRrRE6SkXQ2MXZwgVRqAA5cxsMoEe6UYAXntW7WMQ4mJXQQwUUzwX7MEQITAeVaBWXdFA6EQu2d4A6bMF4ScdXtMeU95XRooUVQwEWpURV$YUJwAVJU4cK9EvXfwc6nNoJmUsXF4u4csE4fUuQXUEY8QeK!wX7zUW6bstN5Y8N9oe5kMFRXgXV9sdYv4VVGofQM4tK!4AQmwE7dpt695tN4sEJ4Ae5npH4CIu4TIEXHUvWD4uYRI8RykFRdVvW4ktUzsE4OIdVSYcS5gvYb5tVEAURqFVIyYd2eMV5T4AJ74WVHIX4vJXJx48YHsvWEgdYRQU4$osRnhVUtAo7cV9N6AuVAMoRW4XYv4eZaxtUCgt6WcfWsQfVaZVQ2IuRSoXX7cHW2QcJTEfQwMfK6@XWOQvNF@t7HMc5qZuJ%EUQmBo7y@vQt@HXswtYm4E4wwcVaAfWwUt7c$tQRYvSqFuS5wWRPoX5CoFR7QE499F7yIAUPUERaFs5HkFJCEf29tvKOEQIb8XRnVXYVwFJr5AIt4dS%MXYFo8KcYQIYoAYvZAJcwUIdht6nBU57A9XtseVr4GVBEpScA95bQeV6EBV6QuSnAvZaIeSS4U7c8FS%IU5%4uYv9fU@Ed5ftAJ9$v5EEv2FAXR2EB4rwXZ5@f6k8vM5kdR4EfRwIQ6Vcf6TwfQ$wtQZQs7a$H2VEGZegvXM4F6aAGQYsoX4wUIcto2ewVZMERY8ooK4YoVCwWJsYUJpcFYUwvU8AcJzYvZw4s6XIU6nNX5wctY%Qu64AEUvAcRDAsVSQuNDgXW2svMeoWQdUsJe@v7cNHR%IQYQovM6MVUF4eZ!IcQmFt68ltZ$MFJeAuX8hF29UdWR8XInxeWZQHSBofX$ktX$MFUHkt4qFQV!8v5Y@f6v5eK7ovJosuWmVuME8vWUEsVnEWQq9HSq@t5OAES6EFInsv6OEX7BwE4tIX7BcVJPQeSDsV7xEd2rRH5nZH5RwUYPYUVHIAUAUBZMUsXUEEIOAu58ZV5s4t6EIAURwUV$4eJzcXZc4A57AeUOwt785HY2AHR3wW66wURqdf7oIvVn@fJPAe5eMu7XIX5WYVXmUtSnlfVRgd5nNUQbNAVREs6Gws4ZUd2$wEZ3YtXFcH4yA8JMsuV8lf5$wUUxMAZ8ZQWoQsZtwUV%IeYpYU5FEQ7MwuZdNWZUsGJdBRU8scQMIAWG8vX!osJeQsN%wdUWws6CQUUwEBS4gfUa5V7QIt49ZHZ2EHS7oE5BsfJBIsVDAXXtAXIUMvXa$FIHQv7VwvWsIU7xEXWtQERd4Q4fJoKEEV4EE82TgfNM8tXesWX8FU7XIW5$A8YsooXWwW4w4f5fEVX$4tZrxFI%scY3gd5y4V6BUdSrsvJyIfXmsA6zEXZvxsQ9AEQWIoJS4uQyEBWo48J9VuKYouVDYeN3oEU%4c5$suN@UHIXoH58JvUbYGZUUsUrtt5nFAR4sU6SsUV6IXR2cV7ZooKegf5bAoKCwW7!wfVQUGQ2s8QkodSrMvWntXQfoc5cJU7aItJXwtVn4f6WsEV6UpKvwF2noHQWEt4MEt62UX5HQU7D4dVGEBIfEuNdVpYsQWQaApK!odN8tQ6kgvXkMtQF8fR3IF49NVWeAFJrcv7cFv6AU8JeJ87%YcNm4HU6IA6CEdVp4oKqVHUo@vQd5d2$wc7vVsKcAv73AXRnNoNTEAJW4dUWsuUOgtXsEtKfEdNqJu5OYEJvoQIG8HYv4UIUwf5f4UYyEXVvAcS2o85DweRm5fUBgX6vlfRa5ER2IQ4EI8Qx@VV8JfUTkdS3@HUFItU9MEIsEFIswX5xAHZrYFVbBQ5vxv5Y8VYBYQJVwGQbgd7xAGZeAQWwQWVTgHJMsV5SIsYmcvNOIeJqweWMEX5Ao8N8BpVDsdYqApVkYG2UAcYUUH7rVvNY4vYPItNfosVGwuV8YeNmNvUfUH7Y8XUdE9NaYc2cZF4YUfQFAVISQEIGYWMtxb#");
        }

        public string Name
        {
            get
            {
                return "Acapela";
            }
        }

        public IEnumerable<string> GetVoices()
        {
            return this.engine.VoiceList;
        }

        public SpeechPartBase CreatePart()
        {
            return new AcapelaSpeechPart();
        }
    }
}