using System;
using System.Collections.Generic;
using UnityEngine;

namespace CooldownManagement {
    /// <summary>
    /// Cooldown management class
    /// Originally created for Hanger Management
    /// Improved for Scenery
    /// Multi-actions support & unscaled time support added for Written In Stone
    /// </summary>
    public class Cooldown {
        private static CooldownManager instance;
        private static CooldownManager globalInstance;

        private static CooldownManager Instance {
            get {
                if (!instance) {
                    GameObject go = new GameObject();
                    go.name = "[Cooldowns Manager]";
                    instance = go.AddComponent<CooldownManager>();
                }

                return instance;
            }
        }

        private static CooldownManager GlobalInstance {
            get {
                if (!globalInstance) {
                    GameObject go = new GameObject();
                    go.name = "[Global Cooldowns Manager]";
                    UnityEngine.Object.DontDestroyOnLoad(go);
                    globalInstance = go.AddComponent<CooldownManager>();
                }

                return globalInstance;
            }
        }

        public static Cooldown Wait(float timeInSeconds) {
            return Wait(timeInSeconds, 1);
        }

        public static Cooldown Wait(float timeInSeconds, float timeModifier) {
            Cooldown cooldown = new Cooldown(timeInSeconds, timeModifier);
            Instance.cooldowns.Add(cooldown);
            return cooldown;
        }

        public static Cooldown WaitUnscaled(float timeInSeconds) {
            Cooldown cooldown = new Cooldown(timeInSeconds, 1, true);
            Instance.cooldowns.Add(cooldown);
            return cooldown;
        }

        public static Cooldown WaitGlobal(float timeInSeconds) {
            Cooldown cooldown = new Cooldown(timeInSeconds);
            GlobalInstance.cooldowns.Add(cooldown);
            return cooldown;
        }

        protected float time;
        protected float elapsed;
        internal List<Action> action;
        protected List<Action> onStop;
        protected List<Action<float, float, float>> onProgress;
        internal List<Action> always;
        private float timeModifier;
        private bool unscaled;

        internal Cooldown(float time, float timeModifier = 1, bool unscaled = false) {
            this.time = time;
            this.elapsed = 0;
            this.timeModifier = timeModifier;
            this.unscaled = unscaled;
            this.action = new();
            this.onStop = new();
            this.onProgress = new();
            this.always = new();
        }

        public Cooldown OnComplete(Action action) {
            this.action.Add(action);
            return this;
        }

        public Cooldown OnProgress(Action<float, float, float> action) {
            this.onProgress.Add(action);
            return this;
        }

        public Cooldown OnProgress(Action<float, float> action) {
            this.onProgress.Add((elapsed, total, delta) => action(elapsed, total));
            return this;
        }

        public Cooldown OnStop(Action action) {
            this.onStop.Add(action);
            return this;
        }

        public Cooldown Always(Action action) {
            this.always.Add(action);
            return this;
        }

        public void Stop() {
            foreach (var action in onStop) {
                action?.Invoke();
            }
            foreach (var action in always) {
                action?.Invoke();
            }
            action.Clear();
            always.Clear(); // Making sure Cooldowns doesn't call it a frame later
            time = elapsed;
        }

        internal bool Progress() {
            var delta = Time.deltaTime;
            if (unscaled) {
                delta = Time.unscaledDeltaTime;
            }

            elapsed += delta * timeModifier;

            if (elapsed > time) {
                elapsed = time;
            }


            foreach (var action in onProgress) {
                action?.Invoke(elapsed, time, delta);
            }
            return Ongoing();
        }

        internal bool Ongoing() {
            return elapsed < time;
        }

        public static bool operator !(Cooldown cd) {
            return cd == null || cd.elapsed >= cd.time;
        }

        public static implicit operator bool(Cooldown cd) {
            return cd != null && cd.elapsed < cd.time;
        }

        private class CooldownManager : MonoBehaviour {
            public List<Cooldown> cooldowns = new();

            private void Update() {
                int i = 0;
                while (i < cooldowns.Count) {
                    if (cooldowns[i].Ongoing() && cooldowns[i].Progress()) {
                        i += 1;
                    } else {
                        foreach (var action in cooldowns[i].action) {
                            action?.Invoke();
                        }
                        foreach (var action in cooldowns[i].always) {
                            action?.Invoke();
                        }
                        cooldowns.RemoveAt(i);
                    }
                }
            }
        }
    }
}
