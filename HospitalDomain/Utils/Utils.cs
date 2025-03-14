namespace Utils
{
    public static class Util
    {
        public static int GetId(IList<int> ids)
        {
            int id = 1;
            for (int i = 0; i < ids.Count; i++)
            {
                if (i + 1 != ids[i])
                {
                    id = i + 1;
                    break;
                }
            }

            if (id == 1 && ids.Count > 0)
            {
                id = ids[^1] + 1;
            } 

            return id;
        }
    }
}