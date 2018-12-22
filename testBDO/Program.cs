namespace testBDO
{
    using System;
    class Program
    {
        static void Main(string[] args)
        {
            string username = "test";
            string password = "test";
            string serverIP = "192.168.11.45";

            if (args.Length == 0)
            {
                //print err
            }
            else
            {
                if (args[0].Length == 0)
                {
                    //print err null username
                }
                else
                {
                    if (args[1].Length == 0)
                    {
                        //print err null password
                    }
                    else
                    {
                        if (args[2].Length == 0)
                        {
                            //print err null serverIP
                        }
                        else
                        {
                            username = args[0];
                            password = args[1];
                            serverIP = args[2];
                        }
                    }
                }
            }

            runGame.LaunchGame(username, password, serverIP);
        } 
    }
}
