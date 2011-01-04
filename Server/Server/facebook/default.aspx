<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >

<head>
    <title>Whist for the soul</title>
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    #silverlightControlHost {
	    height: 100%;
	    text-align:center;
    }
    </style>
 
</head>
<body>
<form runat="server" >
<fb:login-button>
</fb:login-button>
       <div id="fb-root"></div>
       <script src="http://connect.facebook.net/en_US/all.js"></script>
       <script>
           FB.init({ appId: '72325357689', cookie: true, xfbml: true });
       </script>
       <fb:serverFbml width="740px">
          <script type="text/fbml">
          <fb:fbml>
           <fb:request-form method='POST' invite=true
            type='Israeli Whist application'
            content='Would you like to join me in this great waste of time?'>
            <fb:multi-friend-selector cols=3
             actiontext="Invite your friends to waste their time too"
            />
            </fb:request-form>
           </fb:fbml>
           </script>
       </fb:serverFbml>
       <script>
           FB.init({ appId: '72325357689', status: true, cookie: true, xfbml: true });
           FB.Event.subscribe('auth.sessionChange', function (response) {
               if (response.session) {
                   debugger;
                   window.location.href = "http://whist.no-ip.org/wist/facebook/game2.aspx?uid=" + response.session.uid + "&accessToken=" + response.session.access_token;
               } else {
                   // The user has logged out, and the cookie has been cleared
               }
           });
</script>
       </form>
</body>
</html>
