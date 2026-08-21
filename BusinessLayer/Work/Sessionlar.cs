using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public class Sessionlar
{
    public Sessionlar()
    {
    }

    public Sessionlar Current
    {
        get
        {
            Sessionlar session = (Sessionlar)HttpContext.Current.Session["__Sessionlar__"];

            if (session == null)
            {
                session = new Sessionlar();

                HttpContext.Current.Session["__Sessionlar__"] = session;
            }

            return session;
        }
    }

    public CurrentInfo _CurrentInfo
    {
        get
        {
            return (CurrentInfo)
            HttpContext.Current.Session["currentInfo"];
        }

        set
        {
            HttpContext.Current.Session["currentInfo"] = value;
        }
    }
}

