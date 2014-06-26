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
                    var item = Tile.Entities
                        .Where(x => x != Entity)
                        .Where(x => x.HasComponent<InventoryItem>())
                        .FirstOrDefault();

                    var cont = Entity.GetComponentOrNull<Container>();

                    if (item != null && cont != null && cont.CanAddItem(item)) {
                        cont.AddItem(item);
                    }

                    await Delay(ActionLoopPeriod);
                } break;
                case Interaction.ViewInventory: {
                    Client.SendInventoryContents(Entity);
                } break;
            }
        }

        private async void ActionLoop()
        {
            while (IsActive) {
                if (_intent != null && (
                    await Intent.HandleIntentAsync<MoveIntent>(_intent, HandleMoveIntent) ||
                    await Intent.HandleIntentAsync<InteractIntent>(_intent, HandleInteractIntent))) {

                    _intent = null;
                } else {
                    await Delay(ActionLoopPeriod);
                }
            }
        }
    }
}
