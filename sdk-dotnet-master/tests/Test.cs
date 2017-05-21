using System;
using System.Collections;
using NUnit.Framework;
using mercadopago;

namespace MercadoPagoTest {
	
	[TestFixture]
	public class tests {
		MP mp = new MP ("CLIENT_ID", "SECRET_CLIENT");
		
		[Test]
		/* Call preference added through button flow */
		public void getAccessToken() {
			String at = mp.getAccessToken();

			Assert.IsNotNull(at);
		}
		
		[Test]
		/* Call preference added through button flow */
		public void getPreference() {
			String preferenceId = "ID";
			
			Hashtable preference = mp.getPreference(preferenceId);

			Assert.AreEqual(preference["status"], 200);			
			Assert.AreEqual (((Hashtable)preference["response"])["id"], preferenceId);
		}
		
		[Test]
		/* Create a new preference and verify that data result are ok */
		public void createPreference() {
			// String pref = "{'items':[{'title':'create-dotnet','quantity':1,'currency_id':'ARS','unit_price':10.5}]}";
			String pref = "{\"items\":[{\"title\":\"create-dotnet\",\"quantity\":1,\"currency_id\":\"ARS\",\"unit_price\":10.5}]}";

			Hashtable preference = mp.createPreference(pref);

			Assert.AreEqual(preference["status"], 201);
			Assert.AreEqual(((Hashtable)((ArrayList)((Hashtable)preference["response"])["items"])[0])["title"], "create-dotnet");
		}
				
		[Test]
		/* We create a new preference, we modify this one and then we verify that data are ok. */
		public void updatePreference() {
			String pref = "{\"items\":[{\"title\":\"create-dotnet\",\"quantity\":1,\"currency_id\":\"ARS\",\"unit_price\":10.5}]}";
			String updPref = "{\"items\":[{\"title\":\"update-dotnet\",\"quantity\":1,\"currency_id\":\"ARS\",\"unit_price\":10.5}]}";
			
			Hashtable preferenceCreated = mp.createPreference(pref);
			
			Hashtable preferenceUpdate = mp.updatePreference((String)((Hashtable)preferenceCreated["response"])["id"], updPref);
			Assert.AreEqual(preferenceUpdate["status"], 200);
			
			Hashtable preferenceUpdated = mp.getPreference((String)((Hashtable)preferenceCreated["response"])["id"]);
			Assert.AreEqual(((Hashtable)((ArrayList)((Hashtable)preferenceUpdated["response"])["items"])[0])["title"], "update-dotnet");
		}
	}
	
}
