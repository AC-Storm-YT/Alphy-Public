using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Alphy
{
    public static class ProcessCheck
    {
        private const string ProcessName = "RocketLeague";

        public static bool IsGameRunning()
        {
            return Process.GetProcessesByName(ProcessName).Any();
        }

        public static void HandleGameRunning()
        {
            if (IsGameRunning())
            {
                DialogResult result = MessageBox.Show(
                    "Rocket League is currently running. You must close the game to modify or update files.\n\n" +
                    "Would you like Alphy to close the game for you?",
                    "Game Running",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    KillGame();
                }
                else
                {
                    return;
                }
            }
        }

        private static void KillGame()
        {
            try
            {
                var processes = Process.GetProcessesByName(ProcessName);
                foreach (var process in processes)
                {
                    process.Kill();
                    process.WaitForExit(3000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not close the game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
    }
}