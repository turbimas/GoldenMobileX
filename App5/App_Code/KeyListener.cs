using Android.Content;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Text;

public interface OnHomePressedListener
{
    void onHomePressed();
    void onHomeLongPressed();
}

public class HomeWatcher
{
    const String TAG = "hg";
    private Context mContext;
    private IntentFilter mFilter;
    private OnHomePressedListener mListener;
    private InnerReceiver mReceiver;


    public HomeWatcher(Context context)
    {
        mContext = context;
        mFilter = new IntentFilter();
        mFilter.AddAction(Intent.ActionCloseSystemDialogs);
        mFilter.AddAction(Intent.ActionMediaButton);
        mFilter.AddAction("android.media.VOLUME_CHANGED_ACTION");
        mFilter.AddAction(Intent.ActionDial);
        mFilter.AddAction(Intent.ActionCallButton);
        mFilter.AddAction(Intent.ActionView);
        mFilter.AddAction(Intent.ActionCameraButton);
    }

    public void setOnHomePressedListener(OnHomePressedListener listener)
    {
        mListener = listener;
        mReceiver = new InnerReceiver(mListener);
    }

    public void startWatch()
    {
        if (mReceiver != null)
        {
            mContext.RegisterReceiver(mReceiver, mFilter); 
        }
    }

    public void stopWatch()
    {
        if (mReceiver != null)
        {
            mContext.UnregisterReceiver(mReceiver);
        }
    }

    class InnerReceiver : BroadcastReceiver
    {
        const string SYSTEM_DIALOG_REASON_KEY = "reason";
        const string SYSTEM_DIALOG_REASON_GLOBAL_ACTIONS = "globalactions";
        const string SYSTEM_DIALOG_REASON_RECENT_APPS = "recentapps";
        const string SYSTEM_DIALOG_REASON_HOME_KEY = "homekey";
        
        OnHomePressedListener listener;

        public InnerReceiver(OnHomePressedListener listener)
        {
            this.listener = listener;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            if (action.Equals(Intent.ActionCloseSystemDialogs))  //ACTION_CLOSE_SYSTEM_DIALOGS
            {
                String reason = intent.GetStringExtra(SYSTEM_DIALOG_REASON_KEY);
                if (reason != null)
                {
                    Log.Info(TAG, "action:" + action + ",reason:" + reason);
                    if (listener != null)
                    {
                        if (reason.Equals(SYSTEM_DIALOG_REASON_HOME_KEY))
                        {
                            listener.onHomePressed();
                        }
                        else if (reason.Equals(SYSTEM_DIALOG_REASON_RECENT_APPS))
                        {
                            listener.onHomeLongPressed();
                        }
                    }
                }
            }
        }
    }
}