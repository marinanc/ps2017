<%@ Page Language="c#" %>
<%@ Import Namespace="mercadopago" %>
<%@ Import Namespace="System.Collections" %>

    <%
        MP mp = new MP ("CLIENT_ID", "CLIENT_SECRET");

        Hashtable preference = mp.createPreference("{\"items\":[{\"title\":\"sdk-dotnet\",\"quantity\":1,\"currency_id\":\"ARS\",\"unit_price\":10.5}]}");
    %>
    
<!doctype html>
<html>
    <head>
       <title>MercadoPago SDK - Create Preference and Show Checkout Example</title>
    </head>
    <body>
        <a href="<% Response.Write(((Hashtable) preference["response"])["init_point"]); %>" name="MP-Checkout" class="orange-ar-m-sq-arall">Pay</a>
        <script type="text/javascript" src="//resources.mlstatic.com/mptools/render.js"></script>
    </body>
</html>
