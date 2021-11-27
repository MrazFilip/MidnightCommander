namespace MidnightCommander.Components.Subcomponents
{
    class ExceptionHandler
    {
        public string message;
        public ExceptionHandler(System.Exception a)
        {
            if (a.Message.Contains("již existuje."))
                message = "Nelze zadat název souboru, který již ve složce je!";
            else if (a.Message.Contains("jmenovka"))
                message = @"Nelze použít znaky: \ / ?" + '"' + "< > | : *";
            else if(a.Message.Contains("denied"))
                message = "Nelze přistoupit do složky";
            else
                message = "Chybné adresáře!";
        }
    }
}
