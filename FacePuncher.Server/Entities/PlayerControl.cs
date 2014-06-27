/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using System.Threading.Tasks;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Binds control of an entity to a specified client.
    /// </summary>
    class PlayerControl : AgentControl
    {
        const double VisibilityLoopPeriod = 0.25;
        const double ActionLoopPeriod = 0.25;

        /// <summary>
        /// Gets or sets the client that dictates the actions of this entity.
        /// </summary>
        public ClientConnection Client { get; set; }
        private Intent _intent;

        public Intent Intent
        {
            get { return _intent; }
            set { _intent = value; }
        }

        public override void OnPlace()
        {
            Entity.Wake();
        }

        protected override void OnWake()
        {
            VisibilityUpdateLoop();
            ActionLoop();
        }

        private async void VisibilityUpdateLoop()
        {
            while (IsActive) {
                Client.SendVisibleLevelState();
                await Delay(VisibilityLoopPeriod);
            }
        }

        private async Task HandleMoveIntent(MoveIntent intent)
        {
            if (!CanMove(intent.Direction)) return;

            await Move(intent.Direction);
        }

        private async Task HandleInteractIntent(InteractIntent intent)
        {
            switch (intent.Interaction) {
                case Interaction.PickupItem: {
                    var cont = Entity.GetComponentOrNull<Container>();

                    if (cont == null) break;

                    var items = Tile.Entities
                        .Where(x => x != Entity)
                        .Where(x => x.HasComponent<InventoryItem>())
                        .Where(x => cont.CanAddItem(x))
                        .ToArray();

                    if (items.Length == 0) break;

                    if (items.Length == 1) {
                        cont.AddItem(items[0]);
                        break;
                    }

                    Client.SendInventoryContents(Entity, InventoryAction.Pickup);
                } break;
                case Interaction.ViewInventory: {
                    Client.SendInventoryContents(Entity, InventoryAction.None);
                } break;
                case Interaction.EquipItem: {
                    Client.SendInventoryContents(Entity, InventoryAction.Equip);
                } break;
                case Interaction.RemoveItem: {
                    Client.SendInventoryContents(Entity, InventoryAction.Remove);
                } break;
            }

            await Delay(ActionLoopPeriod);
        }

        private async void ActionLoop()
        {
            while (IsActive) {
                if (_intent != null) {
                    var intent = _intent; _intent = null;

                    if (await Intent.HandleIntentAsync<MoveIntent>(intent, HandleMoveIntent) ||
                        await Intent.HandleIntentAsync<InteractIntent>(intent, HandleInteractIntent)) {
                        continue;
                    }
                }

                await Delay(ActionLoopPeriod);
            }
        }
    }
}
