using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.UI;

namespace Sound
{
    // pain from the past: https://github.com/dorev/DryadUnity/blob/main/Assets/Editor/DryadMotifEditor.cs

    public class MusicClipEditor : EditorWindow
    {
        // Audio data members
        MusicClipSO musicClip = null;
        AudioClip audioClip = null;
        float[] samples;
        int frames;
        int channels;
        bool dataLoaded = false;

        // Visual elements
        Rect waveform = new Rect();
        float zoom = 1f;
        float offset;
        float frameSpanRadius;
        float waveformMargin = 40;
        Color waveformColor = Color.green;
        Color framingColor = Color.white;
        Color markerColor = Color.yellow;
        Color errorColor = Color.red;

        [MenuItem("Sound/MusicClip Editor")]
        public static void ShowWindow()
        {
            MusicClipEditor window = GetWindow<MusicClipEditor>();
            window.titleContent = new GUIContent("MusicClip Editor");
        }

        public void LoadMusicClipData(MusicClipSO musicClip)
        {
            samples = null;
            dataLoaded = musicClip && musicClip.GetSamples(ref samples, ref frames, ref channels);
            if(dataLoaded)
            {
                this.musicClip = musicClip;
                audioClip = musicClip.soundData.audioClip;
            }
            else
            {
                this.musicClip = null;
                audioClip = null;
            }
        }

        bool AttemptLoadingSelection()
        {
            OnSelectionChange();
            return dataLoaded;
        }

        private void OnValidate()
        {
            if(musicClip != null && audioClip != musicClip.soundData.audioClip)
            {
                LoadMusicClipData(musicClip);
            }
        }

        private void OnGUI()
        {
            waveform.x = 0;
            waveform.y = waveformMargin;
            waveform.width = position.width;
            waveform.height = position.height - waveformMargin * 2;

            ProcessEvents();

            Handles.BeginGUI();
            Color oldColor = Handles.color;

            if(!dataLoaded && !AttemptLoadingSelection())
            {
                Handles.color = Color.red;
                Handles.DrawLine
                (
                    new Vector3(0, waveform.center.y, 0),
                    new Vector3(waveform.width, waveform.center.y, 0)
                );
                Handles.color = oldColor;
                Handles.EndGUI();
                return;
            }

            Handles.color = framingColor;
            Handles.DrawLine
            (
                new Vector3(0, waveform.y, 0),
                new Vector3(waveform.width, waveform.y, 0)
            );
            Handles.DrawLine
            (
                new Vector3(0, waveform.y + waveform.height, 0),
                new Vector3(waveform.width, waveform.y + waveform.height, 0)
            );
            Handles.DrawLine
            (
                new Vector3(0, waveform.center.y, 0),
                new Vector3(waveform.width, waveform.center.y, 0)
            );

            Handles.color = waveformColor;
            float x = waveform.x;
            float y = waveform.y;
            float barSize = 0;
            float maxBarSize = waveform.height / 2 / channels;
            float channelSize = waveform.height / channels;

            float frameSpanCenter = (float)frames / 2;


            int frameBegin = (int) Mathf.Clamp(frameSpanCenter - frameSpanRadius + offset, 0f, frames);
            int frameEnd = (int) Mathf.Clamp(frameSpanCenter + frameSpanRadius + offset, 0f, frames);
            int frameSpan = frameEnd - frameBegin;
            int packSize = (int) (frameSpan / waveform.width) + 1;

            for(int channel = 0; channel < channels; channel++)
            {
                y = waveform.y + maxBarSize + channelSize * channel;
                Handles.DrawLine
                (
                    new Vector3(0, y, 0),
                    new Vector3(waveform.width, y, 0)
                );

                for (int frame = frameBegin; frame < frameEnd; frame += packSize)
                {
                    x = (float)(frame - frameBegin) / (float)frameSpan * waveform.width;
                    barSize = samples[frame * channels + channel] * maxBarSize;

                    if(barSize > 1f || barSize < -1f)
                    {
                        Handles.DrawLine
                        (
                            new Vector3(x, y, 0),
                            new Vector3(x, y + barSize, 0)
                        );
                    }
                }
            }

            Handles.color = oldColor;
            Handles.EndGUI();
        }

        private void OnSelectionChange()
        {
            dataLoaded = false;

            if (Selection.objects.Length > 0
                && Selection.objects[0] != null
                && Selection.objects[0] is MusicClipSO)
            {
                LoadMusicClipData(Selection.objects[0] as MusicClipSO);
            }

            Repaint();
        }

        // https://github.com/dorev/DryadUnity/blob/main/Assets/Editor/DryadMotifEditor.cs#L272
        void ProcessEvents()
        {
            Event e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                    break;

                case EventType.MouseUp:
                    break;

                case EventType.MouseDrag:
                    //Drag(e.delta);
                    break;

                case EventType.ScrollWheel:
                    Zoom(e.delta);
                    break;
            }
        }

        void Zoom(Vector2 delta)
        {
            zoom += (delta.y / -10f);

            if (zoom < 1f)
            {
                zoom = 1f;
                offset = 0f;
            }

            frameSpanRadius = 1f / zoom * (float)frames / 2;
            GUI.changed = true;
        }

        void Drag(Vector2 delta)
        {
            if(zoom == 1)
            {
                return;
            }
            
            offset += delta.x * -10f / zoom;
            GUI.changed = true;
        }

    }
}
