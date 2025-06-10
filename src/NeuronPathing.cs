using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SSOracleSwarmer;

namespace StunMaster
{
    internal static class NeuronPathing
    {
        internal static MovementMode Controlled = new MovementMode("Controlled", register: true);

        internal static void NeuronControll(SSOracleSwarmer self, Player controller)
        {
            if (self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player && controller.slugcatStats.name.value == "stunmaster")
            {
                float baseSpeed = 5f;
                float accel = 0.9f;

                Vector2 inputDir = Vector2.zero;
                if (controller.input[0].analogueDir.magnitude > 0f)
                {
                    inputDir = controller.input[0].analogueDir.normalized;
                }
                else if (controller.input[0].x != 0 || controller.input[0].y != 0)
                {
                    inputDir = new Vector2(controller.input[0].x, controller.input[0].y).normalized;
                }

                if (inputDir != Vector2.zero)
                {
                    self.mode = Controlled;
                    self.color = new Vector2((float)UnityEngine.Random.Range(0, 3) / 2f, (UnityEngine.Random.value < 0.75f) ? 0f : 1f);
                    self.travelDirection = inputDir;
                    self.firstChunk.vel += inputDir * accel;

                    if (self.firstChunk.vel.magnitude > baseSpeed)
                        self.firstChunk.vel = self.firstChunk.vel.normalized * baseSpeed;

                    self.rotation = Mathf.Atan2(inputDir.y, inputDir.x);
                }
                else if (self.mode == Controlled)self.mode = SSOracleSwarmer.MovementMode.Swarm;
            }
        }

    }
}
