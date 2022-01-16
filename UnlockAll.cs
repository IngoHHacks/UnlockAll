using BepInEx;
using BepInEx.Logging;
using System;
using DiskCardGame;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Configuration;
using System.Linq;

namespace UnlockAll
{

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "IngoH.inscryption.UnlockAll";
        private const string PluginName = "UnlockAll";
        private const string PluginVersion = "1.1.0";

        private static readonly string[] startingDeckNames = new string[] { "Ants", "Bones", "FreeReptiles", "MantisGod", "MooseBlood", "Tentacles", "Vanilla" };

        internal static ManualLogSource Log;

        public bool UnlockImprovedSmoke => Config.Bind(PluginName, "UnlockImprovedSmoke", true, new ConfigDescription("Unlocks the Greater Smoke card.")).Value;
        public bool UnlockClover => Config.Bind(PluginName, "UnlockClover", true, new ConfigDescription("Unlocks the reroll clover.")).Value;
        public bool UnlockExtraLife => Config.Bind(PluginName, "UnlockExtraLife", true, new ConfigDescription("Unlocks the extra life candle. Overrides UnlockClover if true.")).Value;
        public bool UnlockBees => Config.Bind(PluginName, "UnlockBees", false, new ConfigDescription("Unlocks the bee figurine. Overrides UnlockClover and UnlockExtraLife if true.")).Value;
        public bool UnlockProspectorDefeated => Config.Bind(PluginName, "UnlockProspectorDefeated", true, new ConfigDescription("Sets the prospector defeated state to true.")).Value;
        public bool UnlockAnglerDefeated => Config.Bind(PluginName, "UnlockAnglerDefeated", true, new ConfigDescription("Sets the angler defeated state to true.")).Value;
        public bool UnlockTrapperTraderDefeated => Config.Bind(PluginName, "UnlockTrapperTraderDefeated", true, new ConfigDescription("Sets the trapper/trader defeated state to true and unlocks the cheaper pelts.")).Value;
        public bool UnlockFishHook => Config.Bind(PluginName, "UnlockFishHook", true, new ConfigDescription("Unlocks the fishing hook.")).Value;
        public bool UnlockSkink => Config.Bind(PluginName, "UnlockSkink", true, new ConfigDescription("Unlocks the skink card.")).Value;
        public bool UnlockAnts => Config.Bind(PluginName, "UnlockAnts", true, new ConfigDescription("Unlocks the ant cards.")).Value;
        public bool UnlockCagedWolf => Config.Bind(PluginName, "UnlockCagedWolf", true, new ConfigDescription("Unlocks the caged wolf.")).Value;
        public bool UnlockSquirrelTotem => Config.Bind(PluginName, "UnlockSquirrelTotem", true, new ConfigDescription("Unlocks the squirrel totem head.")).Value;
        public bool UnlockWolfCageBroken => Config.Bind(PluginName, "UnlockWolfCageBroken", true, new ConfigDescription("Breaks the wolf cage.")).Value;
        public bool UnlockWolfStatuePlaced => Config.Bind(PluginName, "UnlockWolfStatuePlaced", true, new ConfigDescription("Frees the ritual dagger.")).Value;
        public bool UnlockRitualDagger => Config.Bind(PluginName, "UnlockRitualDagger", true, new ConfigDescription("Unlocks the ritual dagger.")).Value;
        public bool UnlockUsedRitualDagger => Config.Bind(PluginName, "UnlockUsedRitualDagger", true, new ConfigDescription("Sets ritual dagger used state to true.")).Value;
        public bool UnlockRing => Config.Bind(PluginName, "UnlockRing", true, new ConfigDescription("Unlocks the ring.")).Value;
        public bool UnlockTalkingWolf => Config.Bind(PluginName, "UnlockTalkingWolf", true, new ConfigDescription("Unlocks the stunted wolf.")).Value;
        public bool UnlockFilmRoll => Config.Bind(PluginName, "UnlockFilmRoll", false, new ConfigDescription("Unlocks the film roll.")).Value;
        public bool UnlockGooBottle => Config.Bind(PluginName, "UnlockGooBottle", true, new ConfigDescription("Unlocks the goo bottle.")).Value;
        public bool UnlockUhOhSpaghettiOh => Config.Bind(PluginName, "UhOhSpaghettiOh", false, new ConfigDescription("Unlocks the Bone Lord's UhOhSpaghettiOh event.")).Value;
        public bool UnlockPhotographerDroneFound => Config.Bind(PluginName, "UnlockPhotographerDroneFound", false, new ConfigDescription("Sets the photographer drone found state to true.")).Value;
        public bool UnlockTarotCardFound => Config.Bind(PluginName, "UnlockTarotCardFound", false, new ConfigDescription("Sets the cabin tarot card found state to true.")).Value;
        public bool UnlockFailedWithFilmRoll => Config.Bind(PluginName, "FailedWithFilmRoll", false, new ConfigDescription("Sets the failed with film roll state to true.")).Value;
        public bool UnlockSacrificedStoatInTutorial => Config.Bind(PluginName, "UnlockSacrificedStoatInTutorial", false, new ConfigDescription("Effects unknown.")).Value;
        public bool UnlockMisc => Config.Bind(PluginName, "UnlockMisc", true, new ConfigDescription("Unlocks all other Act 1 events.")).Value;
        public bool EnableOuroborosOverride => Config.Bind(PluginName, "EnableOuroborosOverride", false, new ConfigDescription("Whether to override the Ouroboros death count.")).Value;
        public int OuroborosDeathCount => Config.Bind(PluginName, "OuroborosDeathCount", 0, new ConfigDescription("Sets the Ouroboros death count (if OuroborosOverride is true).")).Value;
        public bool EnablePastRunOverride => Config.Bind(PluginName, "PastRunOverride", true, new ConfigDescription("Whether to override the past run count.")).Value;
        public int PastRunCount => Config.Bind(PluginName, "PastRunCount", 4, new ConfigDescription("Sets the past run count (>=4 enables campfire)")).Value;
        public bool UnlockProgressionData => Config.Bind(PluginName, "UnlockProgressionData", true, new ConfigDescription("Sets all progressions (e.g. dialogs, learned cards mechanics) to unlocked.")).Value;
        public bool UnlockAscensionProgression => Config.Bind(PluginName, "UnlockAscensionProgression", true, new ConfigDescription("Unlocks all ascension cards, challenges, starting decks, and dev logs.")).Value;

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");
            Plugin.Log = base.Logger;

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();

            // Create all config values
            Plugin p = new Plugin();
            foreach (PropertyInfo pi in p.GetType().GetProperties())
            {
                if (pi.DeclaringType == typeof(Plugin)) pi.GetValue(p, null);
            }
        }

        [HarmonyPatch(typeof(StartScreenController), "Start")]
        public class StartupPatch : StartScreenController {

            [HarmonyBefore("IngoH.inscryption.SkipStartScreen")]
            public static bool Prefix(StartScreenController __instance)
            {
                if (!startedGame)
                {
                    Log.LogInfo("Unlocking all");
                    Plugin p = new Plugin();
                    UnlockAll(p);
                    SaveFile.IsAscension = !SaveFile.IsAscension;
                    UnlockAll(p);
                    SaveFile.IsAscension = !SaveFile.IsAscension;
                    if (p.UnlockAscensionProgression)
                    {
                        AscensionStoryAndProgressFlags.ITEM_UNLOCK_EVENTS.ForEach(e =>
                        {
                            if (!AscensionSaveData.Data.itemUnlockEvents.Contains(e))
                            {
                                AscensionSaveData.Data.itemUnlockEvents.Add(e);
                            }
                        });
                        AscensionSaveData.Data.oilPaintingState.rewardIndex = 3;
                        AscensionSaveData.Data.oilPaintingState.puzzleSolved = true;
                        AscensionSaveData.Data.oilPaintingState.rewardTaken = true;
                        AscensionSaveData.Data.challengeLevel = 12;
                        AscensionSaveData.Data.conqueredStarterDecks = startingDeckNames.ToList();
                        AscensionSaveData.Data.conqueredChallenges = Enumerable.Range(1, (int)AscensionChallenge.NUM_TYPES).Select(i => (AscensionChallenge)i).ToList();
                    }
                    SaveManager.SaveToFile(false);
                }
                return true;
            }

            private static void UnlockAll(Plugin p)
            {
                if (p.UnlockBees)
                {
                    StoryEventsData.SetEventCompleted(StoryEvent.BeeFigurineFound);
                    SaveManager.SaveFile.oilPaintingState.rewardIndex = 3;
                    SaveManager.SaveFile.oilPaintingState.puzzleSolved = true;
                    SaveManager.SaveFile.oilPaintingState.rewardTaken = true;
                }
                if (p.UnlockExtraLife || SaveManager.SaveFile.oilPaintingState.rewardIndex == 3)
                {
                    StoryEventsData.SetEventCompleted(StoryEvent.CandleArmFound);
                    SaveManager.SaveFile.oilPaintingState.rewardIndex = 3;
                }
                if (p.UnlockClover || SaveManager.SaveFile.oilPaintingState.rewardIndex == 3)
                {
                    StoryEventsData.SetEventCompleted(StoryEvent.CloverFound);
                    SaveManager.SaveFile.oilPaintingState.rewardIndex = 2;
                }
                if (p.UnlockImprovedSmoke)
                {
                    StoryEventsData.SetEventCompleted(StoryEvent.ImprovedSmokeCardDiscovered);
                    SaveManager.SaveFile.wallCandlesProgress = 6;
                }
                if (p.EnablePastRunOverride)
                {
                    for (int i = SaveManager.SaveFile.pastRuns.Count; i < p.PastRunCount; i++)
                    {
                        SaveManager.SaveFile.pastRuns.Add(new RunState());
                    }
                }
                if (p.EnableOuroborosOverride) SaveManager.SaveFile.ouroborosDeaths = p.OuroborosDeathCount;
                if (p.UnlockAnglerDefeated) StoryEventsData.SetEventCompleted(StoryEvent.AnglerDefeated);
                if (p.UnlockAnts) StoryEventsData.SetEventCompleted(StoryEvent.AntCardsDiscovered);
                if (p.UnlockTarotCardFound) StoryEventsData.SetEventCompleted(StoryEvent.CabinTarotCardFound);
                if (p.UnlockCagedWolf) StoryEventsData.SetEventCompleted(StoryEvent.CageCardDiscovered);
                if (p.UnlockFailedWithFilmRoll) StoryEventsData.SetEventCompleted(StoryEvent.FailedWithFilmRoll);
                if (p.UnlockFilmRoll) StoryEventsData.SetEventCompleted(StoryEvent.FilmRollDiscovered);
                if (p.UnlockFishHook) StoryEventsData.SetEventCompleted(StoryEvent.FishHookUnlocked);
                if (p.UnlockGooBottle) StoryEventsData.SetEventCompleted(StoryEvent.GooBottleFound);
                if (p.UnlockPhotographerDroneFound) StoryEventsData.SetEventCompleted(StoryEvent.PhotoDroneSeenInCabin);
                if (p.UnlockProspectorDefeated) StoryEventsData.SetEventCompleted(StoryEvent.ProspectorDefeated);
                if (p.UnlockRing) StoryEventsData.SetEventCompleted(StoryEvent.RingFound);
                if (p.UnlockSacrificedStoatInTutorial) StoryEventsData.SetEventCompleted(StoryEvent.SacrificedStoatInTutorial);
                if (p.UnlockSkink) StoryEventsData.SetEventCompleted(StoryEvent.SkinkCardDiscovered);
                if (p.UnlockRitualDagger) StoryEventsData.SetEventCompleted(StoryEvent.SpecialDaggerDiscovered);
                if (p.UnlockUsedRitualDagger) StoryEventsData.SetEventCompleted(StoryEvent.SpecialDaggerUsed);
                if (p.UnlockSquirrelTotem) StoryEventsData.SetEventCompleted(StoryEvent.SquirrelHeadDiscovered);
                if (p.UnlockMisc)
                {
                    StoryEventsData.SetEventCompleted(StoryEvent.BonesTutorialCompleted);
                    StoryEventsData.SetEventCompleted(StoryEvent.ClockCompartmentOpened);
                    StoryEventsData.SetEventCompleted(StoryEvent.ClockSmallCompartmentOpened);
                    StoryEventsData.SetEventCompleted(StoryEvent.FigurineFetched);
                    StoryEventsData.SetEventCompleted(StoryEvent.LeshyLostCamera);
                    StoryEventsData.SetEventCompleted(StoryEvent.SafeOpened);
                    StoryEventsData.SetEventCompleted(StoryEvent.StinkbugIntroduction2);
                    StoryEventsData.SetEventCompleted(StoryEvent.StinkbugMentionedWolf);
                    StoryEventsData.SetEventCompleted(StoryEvent.StinkbugStoatReunited);
                    StoryEventsData.SetEventCompleted(StoryEvent.StoatIntroduction);
                    StoryEventsData.SetEventCompleted(StoryEvent.StoatIntroduction2);
                    StoryEventsData.SetEventCompleted(StoryEvent.StoatIntroduction3);
                    StoryEventsData.SetEventCompleted(StoryEvent.StoatSaysFindWolf);
                    StoryEventsData.SetEventCompleted(StoryEvent.StoatWolfReunited);
                    StoryEventsData.SetEventCompleted(StoryEvent.TutorialRun2Completed);
                    StoryEventsData.SetEventCompleted(StoryEvent.TutorialRun3Completed);
                    StoryEventsData.SetEventCompleted(StoryEvent.TutorialRunCompleted);
                    StoryEventsData.SetEventCompleted(StoryEvent.WardrobeDrawer1Opened);
                    StoryEventsData.SetEventCompleted(StoryEvent.WardrobeDrawer2Opened);
                    StoryEventsData.SetEventCompleted(StoryEvent.WardrobeDrawer3Opened);
                    StoryEventsData.SetEventCompleted(StoryEvent.WardrobeDrawer4Opened);
                    StoryEventsData.SetEventCompleted(StoryEvent.WardrobePanelOpened);
                    StoryEventsData.SetEventCompleted(StoryEvent.WolfMentionFilmRoll);
                    StoryEventsData.SetEventCompleted(StoryEvent.WoodcarverMet);
                }
                if (p.UnlockTrapperTraderDefeated) StoryEventsData.SetEventCompleted(StoryEvent.TrapperTraderDefeated);
                if (p.UnlockTalkingWolf) StoryEventsData.SetEventCompleted(StoryEvent.TalkingWolfCardDiscovered);
                if (p.UnlockUhOhSpaghettiOh) StoryEventsData.SetEventCompleted(StoryEvent.UhOhSpaghettiOh);
                if (p.UnlockWolfCageBroken) StoryEventsData.SetEventCompleted(StoryEvent.WolfCageBroken);
                if (p.UnlockWolfStatuePlaced) StoryEventsData.SetEventCompleted(StoryEvent.WolfStatuePlaced);
                if (p.UnlockProgressionData)
                {
                    for (int i = 1; i < (int)Ability.NUM_ABILITIES; i++)
                    {
                        ProgressionData.SetAbilityLearned((Ability)i);
                    }
                    for (int j = 0; j < (int)MechanicsConcept.NUM_MECHANICS; j++)
                    {
                        ProgressionData.SetMechanicLearned((MechanicsConcept)j);
                    }
                    foreach (CardInfo allDatum in ScriptableObjectLoader<CardInfo>.AllData)
                    {
                        ProgressionData.SetCardLearned(allDatum);
                        ProgressionData.SetCardIntroduced(allDatum);
                    }
                }
            }
        }
    }
}
