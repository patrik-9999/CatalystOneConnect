using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace APIs;

public class CatalystAccess
{
    public string access_token { get; set; } = string.Empty;
    public string refresh_token { get; set; } = string.Empty;
    public long expires_in { get; set; }
    public long refresh_token_expires_in { get; set; }
}
