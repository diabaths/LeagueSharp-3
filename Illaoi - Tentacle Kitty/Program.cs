﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Illaoi___Tentacle_Kitty
{
    class Program
    {
        public static Spell Q,W,E,R;
        private static readonly Obj_AI_Hero Illaoi = ObjectManager.Player;
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;


        public static string[] HighChamps =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs","Kindred"
            };

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Illaoi.ChampionName != "Illaoi")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(.484f, 0, 500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(.066f, 50, 1900, true, SkillshotType.SkillshotLine);

            Config = new Menu("Illaoi - Tentacle Kitty", "Illaoi - Tentacle Kitty", true);
            {
                TargetSelector.AddToMenu(Config.SubMenu("Target Selector Settings"));
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.combo", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.min.hit", "(R) Min. Hit").SetValue(new Slider(3, 1, 5)));
                    Config.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("Harass Settings", "Combo Settings");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("w.harass", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "Use E").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    Config.AddSubMenu(harassMenu);
                }

                var clearMenu = new Menu("Clear Settings", "Clear Settings");
                {
                    clearMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                    clearMenu.AddItem(new MenuItem("clear.mana", "Mana Manager").SetValue(new Slider(20, 1, 99)));
                    Config.AddSubMenu(clearMenu);
                }

                var eMenu = new Menu("E Settings", "E Settings");
                {
                    eMenu.AddItem(new MenuItem("e.whte", "                     E Whitelist"));
                    foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(o => o.IsEnemy))
                    {
                        eMenu.AddItem(new MenuItem("enemy." + enemy.CharData.BaseSkinName, string.Format("E: {0}", enemy.CharData.BaseSkinName)).SetValue(HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    Config.AddSubMenu(eMenu);
                }

                /*var ksMenu = new Menu("KillSteal Settings", "KillSteal Settings");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "Use Q").SetValue(true));
                    Config.AddSubMenu(ksMenu);
                }*/
                var drawMenu = new Menu("Draw Settings", "Draw Settings");
                {
                    var damageDraw = new Menu("Damage Draw", "Damage Draw");
                    {
                        damageDraw.AddItem(new MenuItem("aa.indicator", "AA Indicator").SetValue(new Circle(true, Color.Gold)));
                        drawMenu.AddSubMenu(damageDraw);
                    }
                    drawMenu.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, Color.White)));
                    drawMenu.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, Color.Gold)));
                    drawMenu.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, Color.DodgerBlue)));
                    drawMenu.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, Color.GreenYellow)));
                    Config.AddSubMenu(drawMenu);
                }
                Config.AddToMainMenu();
            }
            Game.PrintChat("<font color='#ff3232'>Illaoi - Tentacle Kitty: </font> <font color='#d4d4d4'>If you like this assembly feel free to upvote on Assembly DB</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("q.draw").GetValue<Circle>();
            var menuItem2 = Config.Item("w.draw").GetValue<Circle>();
            var menuItem3 = Config.Item("e.draw").GetValue<Circle>();
            var menuItem4 = Config.Item("r.draw").GetValue<Circle>();
            var menuItem5 = Config.Item("aa.indicator").GetValue<Circle>();
            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Illaoi.Position.X, Illaoi.Position.Y, Illaoi.Position.Z), Q.Range, menuItem1.Color, 5);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Illaoi.Position.X, Illaoi.Position.Y, Illaoi.Position.Z), W.Range, menuItem2.Color, 5);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Illaoi.Position.X, Illaoi.Position.Y, Illaoi.Position.Z), E.Range, menuItem3.Color, 5);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Illaoi.Position.X, Illaoi.Position.Y, Illaoi.Position.Z), R.Range, menuItem4.Color, 5);
            }
            if (menuItem4.Active)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(1500) && x.IsValid && x.IsVisible && !x.IsDead && !x.IsZombie))
                {
                    Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y, menuItem5.Color,
                                        string.Format("{0} Basic Attack = Kill", AaIndicator(enemy)));
                }
            }
        }
        private static int AaIndicator(Obj_AI_Hero enemy)
        {
            double aCalculator = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical, Illaoi.TotalAttackDamage());
            double killableAaCount = enemy.Health / aCalculator;
            int totalAa = (int)Math.Ceiling(killableAaCount);
            return totalAa;
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }
        }
        private static void Combo()
        {
            if (Q.IsReady() && Config.Item("q.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            if (W.IsReady() && Config.Item("w.combo").GetValue<bool>())
            {
                var tentacles =
                    ObjectManager.Get<Obj_AI_Base>().Where(x => x.Name.ToLower().Contains("illaoiminion")).FirstOrDefault();
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.Distance(tentacles.Position) < Q.Range))
                {
                    W.Cast();
                }

            }
            if (E.IsReady() && Config.Item("e.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Config.Item("enemy." + enemy.CharData.BaseSkinName).GetValue<bool>())
                    {
                        E.Cast(enemy);
                    }
                } 
            }
            if (R.IsReady() && Config.Item("r.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(R.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Illaoi.CountEnemiesInRange(R.Range) >= Config.Item("r.min.hit").GetValue<Slider>().Value)
                    {
                        R.Cast();
                    }
                } 
            }
        }
        private static void Harass()
        {
            if (Illaoi.ManaPercent < Config.Item("harass.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Q.IsReady() && Config.Item("q.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (Q.GetPrediction(enemy).Hitchance >= HitChance.High)
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            if (W.IsReady() && Config.Item("w.harass").GetValue<bool>())
            {
                var tentacles =
                    ObjectManager.Get<Obj_AI_Base>().Where(x => x.Name.ToLower().Contains("aoi")).FirstOrDefault();
                foreach (var enemy in HeroManager.Enemies.Where(x => x.Distance(tentacles.Position) < Q.Range))
                {
                    W.Cast();
                }

            }
            if (E.IsReady() && Config.Item("e.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Config.Item("enemy." + enemy.CharData.BaseSkinName).GetValue<bool>())
                    {
                        E.Cast(enemy);
                    }
                }
            }
        }
        private static void Clear()
        {
            if (Illaoi.ManaPercent < Config.Item("clear.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Q.IsReady() && Config.Item("q.combo").GetValue<bool>())
            {
                var minionQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
                var lineLocation = Q.GetCircularFarmLocation(minionQ);
                if (lineLocation.MinionsHit >= Config.Item("q.minion.hit").GetValue<Slider>().Value)
                {
                    Q.Cast(lineLocation.Position);
                }
            }
        }
    }
}
