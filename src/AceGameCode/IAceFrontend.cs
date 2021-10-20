using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apian;

namespace AceGameCode
{
    public enum MessageSeverity { Info, Warning, Error };

    public interface IAceFrontend
    {
        IAceApplication AceAppl {get;}
        IAceAppCore AppCore {get;}

        void SetAceApplication(IAceApplication application);
        void AddAppCore(IAceAppCore core);
        AceUserSettings GetUserSettings();

        void DisplayMessage(MessageSeverity level, string msgText);
        Task<GameSelectedEventArgs> SelectGameAsync(IDictionary<string, AceGameAnnounceData> existingGames);
    }
}