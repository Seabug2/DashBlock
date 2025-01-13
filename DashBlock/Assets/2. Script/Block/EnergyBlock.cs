public class EnergyBlock : Block
{
    public override void TakeDamage(Block HitBlock = null)
    {
        if (HitBlock != null && HitBlock is DashBlock dashBlock)
        {
            dashBlock.HP = 99;
            HP = 0;
        }
    }
}
