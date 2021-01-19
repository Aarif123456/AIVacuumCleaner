dddnamespace GameBrains.Entities
{
    public class TargetEntity : Entity
    {
        public float value; // how valuable is this target

        public override void Update()
        {
            base.Update();

            // TODO: could vary value over time, etc.
        }
    }
}