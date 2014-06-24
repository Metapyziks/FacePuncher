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

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Binds control of an entity to a specified client.
    /// </summary>
    class PlayerControl : AgentControl
    {
        const double VisibilityLoopPeriod = 0.25;
        const double MovementLoopPeriod = 0.25;

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
            MovementLoop();
        }

        private async void VisibilityUpdateLoop()
        {
            while (IsActive) {
                Client.SendVisibleLevelState();
                await Delay(VisibilityLoopPeriod);
            }
        }

        private async void MovementLoop()
        {
            while (IsActive) {
                var direc = Direction.None;
                var action = Interaction.None;

                if (Intent.HandleIntent<MoveIntent>(ref _intent, x => direc = x.Direction) && CanMove(direc)) {
                    await Move(direc);
                } else if (Intent.HandleIntent<InteractIntent>(ref _intent, x => action = x.Interaction)) {
                    switch (action) {
                        case Interaction.PickupItem:
                            var item = Tile.Entities
                                .Where(x => x != Entity)
                                .Where(x => x.HasComponent<InventoryItem>())
                                .FirstOrDefault();

                            var cont = Entity.GetComponentOrNull<Container>();

                            if (item != null && cont != null && cont.CanAddItem(item)) {
                                cont.AddItem(item);
                            }
                            
                            break;
                    }
                } else {
                    await Delay(MovementLoopPeriod);
                }
            }
        }
    }
}
