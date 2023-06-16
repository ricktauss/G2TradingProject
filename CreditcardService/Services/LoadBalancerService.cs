namespace IEGEasyCreditcardService.Services
{
    public class LoadBalancerService
    {
        private Dictionary<string,int> controllerBalance = new Dictionary<string,int>();

        public LoadBalancerService() { 

        }

        public int GetBalance(string name)
        {
            if(controllerBalance.ContainsKey(name))
            {
                int actBalance = controllerBalance[name];
                controllerBalance[name]++;

                return actBalance;

            } else
            {
                controllerBalance.Add(name, 1);
                return 0;
            }
        }

        public int SetBalance(string name, int value)
        {
            if(controllerBalance.ContainsKey(name))
            {
                controllerBalance[name] = value;
            } else
            {
                controllerBalance.Add(name, value);
            }

            return GetBalance(name);
        }
    }
}
