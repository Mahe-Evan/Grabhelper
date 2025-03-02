using Celeste.Mod.Grabhelper;

namespace Celeste.Mod.Grabhelper {
    public static class GrabCountJournalPage {
        public static void Load() {
            Everest.Events.Journal.OnEnter += onJournalEnter;
        }

        public static void Unload() {
            Everest.Events.Journal.OnEnter -= onJournalEnter;
        }

        private static void onJournalEnter(OuiJournal journal, Oui from) {
            // add the "dashes" page just after the "deaths" one
            for (int i = 0; i < journal.Pages.Count; i++) {
                if (journal.Pages[i].GetType() == typeof(OuiJournalDeaths)) {
                    journal.Pages.Insert(i + 1, new SaveDisplayInJournal(journal, "journal_grabhelper"));
                }
            }
        }

    }
}