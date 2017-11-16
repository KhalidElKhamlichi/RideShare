package md5f731d8e3af5beae8837e502f7b9d3f07;


public class AddDemandActivity
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Ride_Share.AddDemandActivity, Ride Share, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AddDemandActivity.class, __md_methods);
	}


	public AddDemandActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AddDemandActivity.class)
			mono.android.TypeManager.Activate ("Ride_Share.AddDemandActivity, Ride Share, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
