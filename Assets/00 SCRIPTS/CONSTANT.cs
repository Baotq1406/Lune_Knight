public static class CONSTANT 
{
    public static string PLAYER_TAG = "Player"; // tag cua player

    #region Enemy Tags
    public static string ENEMY_TAG = "Enemy"; // tag cua enemy
    public static string ENEMY_ORC_TAG = "Orc_Enemy"; 
    public static string ENEMY_ARCHER_TAG = "Archer_Enemy";
    public static string ENEMY_BOSS_TAG = "Boss_Enemy";
    #endregion 

    #region Enemy Animator Parameters
    public static string IS_RUNNING = "isRunning"; // animation chay
    // Melee Attack Enemy Animations
    public static string MELEE_ATTACK = "meleeAttack"; // animation tan cong
    public static string MELEE_HURT = "meleeHurt"; // animation bi thuong
    public static string MELEE_DEATH = "meleeDeath"; // animation chet
    // Ranged Attack Enemy Animations
    public static string RANGED_ATTACK = "rangedAttack"; // animation tan cong
    public static string RANGED_HURT = "rangedHurt"; // animation bi thuong
    public static string RANGED_DEATH = "rangedDeath"; // animation chet

    //boss animations
    public static string BOSS_MELEE_ATK = "MELEEATK"; // animation tan cong melee boss
    public static string BOSS_MELEE_ATK_2 = "MELEEATK_2"; // animation tan cong melee 2 boss
    public static string BOSS_RANGED_ATK = "RANGEDATK"; // animation tan cong ranged boss (trigger)
    public static string BOSS_HURT = "HURT"; // animation bi thuong boss (trigger)
    public static string BOSS_DEAD = "DEAD"; // animation chet boss (trigger)
    #endregion

    #region Player Stats
    public static string POINTS = "UpgradePoints"; // diem nang cap cua player
    public static string MAX_HEALTH = "MaxHealth"; // mau toi da cua player
    public static string ATTACK_DAMAGE = "AttackDamage"; // sat thuong tan cong cua player
    public static string HEAL_AMOUNT = "HealAmount"; // luc hoi mau cua player
    public static string LAST_CHECKPOINT_HEALTH = "LastCheckpointHealth"; // mau luc luu checkpoint
    #endregion
}
