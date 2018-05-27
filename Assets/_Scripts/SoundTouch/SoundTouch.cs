using System;
using System.Runtime.InteropServices;
using System.Text;

class SoundTouch : IDisposable
{
    private IntPtr handle;
    private string versionString;
    private readonly bool is64Bit;
    public SoundTouch()
    {
		is64Bit = Marshal.SizeOf(typeof(IntPtr)) == 8;
		UnityEngine.Debug.Log (is64Bit);
		handle = SoundTouchInterop.soundtouch_createInstance ();
    }

    public string VersionString
    {
        get
        {
            if (versionString == null)
            {
                var s = new StringBuilder(100);
                SoundTouchInterop.soundtouch_getVersionString2(s, s.Capacity);
                versionString = s.ToString();
            }
            return versionString;
        }
    }

    public void SetPitchOctaves(float pitchOctaves)
    {
        SoundTouchInterop.soundtouch_setPitchOctaves(handle, pitchOctaves);
    }

    public void SetSampleRate(int sampleRate)
    {
        SoundTouchInterop.soundtouch_setSampleRate(handle, (uint) sampleRate);
    }

    public void SetChannels(int channels)
    {
        SoundTouchInterop.soundtouch_setChannels(handle, (uint)channels);
    }

    private void DestroyInstance()
    {
        if (handle != IntPtr.Zero)
        {
            SoundTouchInterop.soundtouch_destroyInstance(handle);
            handle = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        DestroyInstance();
        GC.SuppressFinalize(this);
    }

    ~SoundTouch()
    {
        DestroyInstance();
    }

    public void PutSamples(float[] samples, int numSamples)
    {
        SoundTouchInterop.soundtouch_putSamples(handle, samples, numSamples);
    }

    public int ReceiveSamples(float[] outBuffer, int maxSamples)
    {
        return (int)SoundTouchInterop.soundtouch_receiveSamples(handle, outBuffer, (uint)maxSamples);
    }

    public bool IsEmpty
    {
        get
        {
            return SoundTouchInterop.soundtouch_isEmpty(handle) != 0;
        }
    }

    public int NumberOfSamplesAvailable
    {
        get
        {
            return (int)SoundTouchInterop.soundtouch_numSamples(handle);
        }
    }

    public int NumberOfUnprocessedSamples
    {
        get
        {
            return SoundTouchInterop.soundtouch_numUnprocessedSamples(handle);
        }
    }

    public void Flush()
    {
        SoundTouchInterop.soundtouch_flush(handle);
    }

    public void Clear()
    {
        SoundTouchInterop.soundtouch_clear(handle);
    }

    public void SetRate(float newRate)
    {
        SoundTouchInterop.soundtouch_setRate(handle, newRate);
    }

    public void SetTempo(float newTempo)
    {
        SoundTouchInterop.soundtouch_setTempo(handle, newTempo);
    }

    public int GetUseAntiAliasing()
    {
        return SoundTouchInterop.soundtouch_getSetting(handle, SoundTouchSettings.UseAaFilter);
    }

    public void SetUseAntiAliasing(bool useAntiAliasing)
    {
        SoundTouchInterop.soundtouch_setSetting(handle, SoundTouchSettings.UseAaFilter, useAntiAliasing ? 1 : 0);
    }

    public void SetUseQuickSeek(bool useQuickSeek)
    {
        SoundTouchInterop.soundtouch_setSetting(handle, SoundTouchSettings.UseQuickSeek, useQuickSeek ? 1 : 0);
    }

    public int GetUseQuickSeek()
    {
        return SoundTouchInterop.soundtouch_getSetting(handle, SoundTouchSettings.UseQuickSeek);
    }
}
