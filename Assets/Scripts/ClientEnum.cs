namespace ClientEnum
{
    public enum GameStatus
    {
        Selection,
        Play,
        Running,
        Clear,
        Fail
    }

    public enum GameMode
	{
		Normal = 1,
		Boss,
    }

	public enum Bubble
	{ 
		Normal = 1,
		Special,
        NoSet,
	}

    public enum BubbleProperty
    {
        HitBoss = 1,
        CollisionBubble,
        Rescue
    }
}