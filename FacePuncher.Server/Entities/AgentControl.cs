using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    class AgentControl : Component
    {
        private ulong _nextMove;

        [ScriptDefinable]
        public ulong MovePeriod { get; set; }

        protected bool CanMove(ulong time)
        {
            return time >= _nextMove;
        }

        protected void Move(Direction dir, ulong time)
        {
            if (MovePeriod == 0 || !CanMove(time) || !Entity.CanMove(dir)) return;

            Entity.Move(dir);
            _nextMove += MovePeriod;
        }
    }
}
