using System.Collections;
using System.Collections.Generic;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using _01.Scripts.Fish;
using _01.Scripts.Manager.Fishing;
using _01.Scripts.SaveSystem;
using SaveDataSystem = _01.Scripts.SaveSystem.SaveSystem;
using _01.Scripts.Sounds;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01.Scripts.Manager
{
    public class FishingManager : MonoBehaviour
    {
        [Header("Wait Setting")] [Range(1f, 15f), SerializeField]
        private float minWaitFishingTime = 1f;

        [Range(1f, 15f), SerializeField] private float maxWaitFishingTime = 4f;
        [SerializeField] private float delayTime = 2f;

        [Header("Damage Setting")] [SerializeField]
        private int damageTime = 5;

        [SerializeField] private int levelUpDamageTime = 2;

        [Header("Fish")] [SerializeField] private FishDataListSO fishDataList;
        [SerializeField] private List<int> fishingRandomPercent;

        [Header("Channels")] [SerializeField] private GameEventChannelSO fishingUiChannel;
        [SerializeField] private GameEventChannelSO fishingChannel;
        [SerializeField] private GameEventChannelSO resultUiChannel;
        [SerializeField] private GameEventChannelSO inventoryChannel;
        [SerializeField] private GameEventChannelSO mapChannel;
        [SerializeField] private GameEventChannelSO soundChannel;

        [Header("Sound")] [SerializeField] private SoundSO reelSound;

        private FishingSessionState _session;

        private FishDataSO _currentFishData;
        private int _currentMapIndex;
        private Coroutine _waitingFishingCoroutine;

        private void Awake()
        {
            _session = new FishingSessionState();

            fishingChannel.AddListener<LevelUpEvent>(HandleLevelUp);
            fishingChannel.AddListener<FishingCommandEvent>(HandleFishingCommand);
            mapChannel.AddListener<ChangeMapEvent>(HandleMapChanged);

            LoadSavedFishingData();
        }

        private void OnDestroy()
        {
            fishingChannel.RemoveListener<LevelUpEvent>(HandleLevelUp);
            fishingChannel.RemoveListener<FishingCommandEvent>(HandleFishingCommand);
            mapChannel.RemoveListener<ChangeMapEvent>(HandleMapChanged);

            if (_waitingFishingCoroutine != null)
            {
                StopCoroutine(_waitingFishingCoroutine);
                _waitingFishingCoroutine = null;
            }
        }

        private void Update()
        {
            if (_currentFishData == null) return;

            if (_session.IsFishingStarted)
            {
                _session.UpdatePerfectZone(Time.deltaTime);
                fishingUiChannel.RaiseEvent(
                    FishingUIEvent.CreateChangePerfectZoneEvent(_session.PerfectZoneMin, _session.PerfectZoneMax)
                );

                bool isInPerfectZone = _session.IsReelInPerfectZone();
                if (_session.TryConsumePerfectZoneEntryChanged(isInPerfectZone, out bool shouldShowPerfectText))
                {
                    fishingUiChannel.RaiseEvent(FishingUIEvent.CreateTogglePerfectTextEvent(shouldShowPerfectText));
                }
            }

            if (!_session.IsReeling) return;

            _session.UpdateReel(Time.deltaTime);
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateChangeReelGaugeEvent(_session.ReelGauge));

            _session.UpdateFishingTime(Time.deltaTime);
            if (_session.TryApplyPerfectZoneDamage(Time.deltaTime, damageTime, _currentFishData.hp,
                    out float hpPercent))
            {
                fishingUiChannel.RaiseEvent(FishingUIEvent.CreateChangeFishHpEvent(hpPercent));
            }

            if (_session.IsFailed(_currentFishData.fishingTime))
            {
                EndFishingRound();
                return;
            }

            if (_session.IsFishCaught())
            {
                HandleFishCaught();
            }
        }

        private void ReelGaugeUp()
        {
            _session.ReelGaugeUp();
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateRollingReelEvent(true));
        }

        private void ReelGaugeDown()
        {
            _session.ReelGaugeDown();
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateRollingReelEvent(false));
        }

        private void StartFishing()
        {
            if (_currentFishData == null)
            {
                Debug.LogWarning("FishingManager: StartFishing called before fish selection.");
                return;
            }

            if (reelSound != null)
            {
                soundChannel.RaiseEvent(
                    SoundEvent.CreatePlayLongSfxEvent(transform.position, reelSound, reelSound.name)
                );
            }

            _session.StartFishing(_currentFishData);
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateTogglePerfectTextEvent(false));
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateTimerDownEvent(_currentFishData.fishingTime));
            fishingUiChannel.RaiseEvent(
                FishingUIEvent.CreateChangePerfectZoneEvent(_session.PerfectZoneMin, _session.PerfectZoneMax)
            );
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateStartFishingEvent());
        }

        private void StopReeling()
        {
            _session.StopReeling();
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateStopReelingEvent());
        }

        private IEnumerator WaitingFishing()
        {
            float randomWait = Random.Range(minWaitFishingTime, maxWaitFishingTime);
            yield return new WaitForSeconds(randomWait);

            _currentFishData = fishDataList?.GetRandomFishByMap(_currentMapIndex, fishingRandomPercent);
            if (_currentFishData == null)
            {
                Debug.LogWarning("FishingManager: No available fish for current map.");
                _waitingFishingCoroutine = null;
                yield break;
            }

            fishingChannel.RaiseEvent(FishingEvent.CreateSignalEvent(FishingSignalType.HitFish));
            yield return new WaitForSeconds(delayTime);
            fishingChannel.RaiseEvent(FishingEvent.CreateSignalEvent(FishingSignalType.MissFish));

            _waitingFishingCoroutine = null;
        }

        private void HandleFishCaught()
        {
            int fishSize = Random.Range(_currentFishData.size.x, _currentFishData.size.y);
            bool isBestRecord = SaveCaughtFish(_currentFishData, fishSize);

            resultUiChannel.RaiseEvent(ResultUIEvent.CreateShowResultUIEvent(
                _currentFishData.fishSprite,
                _currentFishData.fishName,
                _currentFishData.tier.ToString(),
                _currentFishData.backTierImageColor,
                fishSize,
                isBestRecord,
                _currentFishData.spriteSize,
                _currentFishData.isFlip));

            inventoryChannel.RaiseEvent(InventoryEvent.CreateStackFishEvent(_currentFishData));
            EndFishingRound();
        }

        private void EndFishingRound()
        {
            if (reelSound != null)
            {
                soundChannel.RaiseEvent(SoundEvent.CreateStopLongSfxEvent(reelSound.name));
            }

            _session.Reset();
            fishingChannel.RaiseEvent(FishingEvent.CreateSignalEvent(FishingSignalType.RoundEnded));
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateStopReelingEvent());
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateTogglePerfectTextEvent(false));
            fishingUiChannel.RaiseEvent(FishingUIEvent.CreateEndFishingEvent());
        }

        private void HandleLevelUp(LevelUpEvent evt)
        {
            damageTime += levelUpDamageTime;
            GameSaveData data = SaveDataSystem.Load();
            data.damageTime = damageTime;
            SaveDataSystem.Save(data);
        }

        private void HandleFishingCommand(FishingCommandEvent evt)
        {
            switch (evt.commandType)
            {
                case FishingCommandType.StartWaiting:
                    if (_waitingFishingCoroutine != null)
                    {
                        StopCoroutine(_waitingFishingCoroutine);
                    }

                    _waitingFishingCoroutine = StartCoroutine(WaitingFishing());
                    break;
                case FishingCommandType.StopWaiting:
                    if (_waitingFishingCoroutine != null)
                    {
                        StopCoroutine(_waitingFishingCoroutine);
                        _waitingFishingCoroutine = null;
                    }

                    break;
                case FishingCommandType.StartFishing:
                    StartFishing();
                    break;
                case FishingCommandType.ReelGaugeUp:
                    ReelGaugeUp();
                    break;
                case FishingCommandType.ReelGaugeDown:
                    ReelGaugeDown();
                    break;
                case FishingCommandType.StopReeling:
                    StopReeling();
                    break;
            }
        }

        private void HandleMapChanged(ChangeMapEvent evt)
        {
            _currentMapIndex = evt.mapIndex;
        }

        private void LoadSavedFishingData()
        {
            GameSaveData data = SaveDataSystem.Load();
            data.fishCaughtStates ??= new Dictionary<string, bool>();
            data.fishMaxSizes ??= new Dictionary<string, int>();

            bool changed = EnsureFishCaughtStateEntries(data);
            if (changed)
            {
                SaveDataSystem.Save(data);
            }

            _currentMapIndex = data.mapIndex > 0 ? data.mapIndex : 1;
            damageTime = data.damageTime != 0 ? data.damageTime : damageTime;
        }

        private bool SaveCaughtFish(FishDataSO fishData, int fishSize)
        {
            if (fishData == null) return false;

            GameSaveData data = SaveDataSystem.Load();
            data.fishCaughtStates ??= new Dictionary<string, bool>();
            data.fishMaxSizes ??= new Dictionary<string, int>();

            string fishKey = fishData.fishName;
            int previousMax = data.fishMaxSizes.GetValueOrDefault(fishKey, 0);
            bool isBestRecord = fishSize > previousMax;

            data.fishCaughtStates[fishKey] = true;
            if (isBestRecord)
            {
                data.fishMaxSizes[fishKey] = fishSize;
            }

            SaveDataSystem.Save(data);
            return isBestRecord;
        }

        private bool EnsureFishCaughtStateEntries(GameSaveData data)
        {
            if (fishDataList == null || fishDataList.allFishDatas == null)
            {
                return false;
            }

            bool changed = false;
            foreach (FishDataSO fish in fishDataList.allFishDatas)
            {
                if (fish == null) continue;
                if (!data.fishCaughtStates.TryAdd(fish.fishName, false)) continue;

                changed = true;
            }

            return changed;
        }
    }
}