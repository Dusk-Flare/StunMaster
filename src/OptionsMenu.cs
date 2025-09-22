using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StunMaster
{
    public class OptionsMenu : OptionInterface
    {
        public readonly Configurable<bool> LPC;

        private UIelement[] options;

        public OptionsMenu()
        {
            LPC = config.Bind("LPC", false);
        }

        public override void Initialize()
        {
            OpTab optionsTab = new(this, "Options");
            Tabs = [optionsTab];

            options = [
                new OpCheckBox(LPC, new Vector2(0f, 475f)),
                new OpLabel(new Vector2(0f, 500f), new Vector2(600f, 30f), "Legacy Stunmaster Powers?", FLabelAlignment.Left, true),
            ];

            optionsTab.AddItems(options);
        }
    }
}
