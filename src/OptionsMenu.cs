using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StunFall
{
    public class OptionsMenu : OptionInterface
    {
        public readonly Configurable<bool> DSI;
        public readonly Configurable<bool> LGP;

        private UIelement[] options;

        public OptionsMenu()
        {
            DSI = config.Bind("DSI", true);
            LGP = config.Bind("LGP", false);
        }

        public override void Initialize()
        {
            OpTab optionsTab = new(this, "Options");
            Tabs = [optionsTab];

            options = [
                new OpCheckBox(DSI, new Vector2(0f, 475f)),
                new OpLabel(new Vector2(0f, 500f), new Vector2(600f, 30f), "Drop Story Items?", FLabelAlignment.Left, true),

                new OpCheckBox(LGP, new Vector2(0f, 375f)),
                new OpLabel(new Vector2(0f, 400f), new Vector2(600f, 30f), "Disable Lizard Grace Period?", FLabelAlignment.Left, true),
            ];

            optionsTab.AddItems(options);
        }
    }
}
