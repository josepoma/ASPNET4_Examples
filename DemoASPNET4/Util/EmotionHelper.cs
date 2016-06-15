using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using System.IO;
using Microsoft.ProjectOxford.Emotion.Contract;
using DemoASPNET4.Models;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DemoASPNET4.Util
{
    public class EmotionHelper
    {
        public EmotionServiceClient emoClient;

        public EmotionHelper(string key)
        {
            emoClient = new EmotionServiceClient(key);
        }

        public async Task<EmoPicture> DetectAndExtractFacesAsync(Stream imageStream)
        {
            Emotion[] emotions =  await emoClient.RecognizeAsync(imageStream);

            var emoPicture = new EmoPicture();

            emoPicture.Faces = ExtractFaces(emotions, emoPicture);

            return emoPicture;
        }

        private ObservableCollection<EmoFace> ExtractFaces(Emotion[] emotions, EmoPicture emoPicture)
        {
            var listaFaces = new ObservableCollection<EmoFace>();

            foreach (var item in emotions)
            {
                var emoface = new EmoFace()
                {
                    X = item.FaceRectangle.Left,
                    Y = item.FaceRectangle.Top,
                    Width = item.FaceRectangle.Width,
                    Height = item.FaceRectangle.Height,
                    Picture = emoPicture
                };                

                emoface.Emotions = ProcessEmotions(item.Scores, emoface);
                listaFaces.Add(emoface);
            }

            return listaFaces;
            
        }

        private ObservableCollection<EmoEmotion> ProcessEmotions(Scores scores, EmoFace emoface)
        {
            var emotionList = new ObservableCollection<EmoEmotion>();

            var properties = scores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var filterProperties = properties.Where(p => p.PropertyType == typeof(float));

            var emotype = EmoEmotionEnum.Undetermined;

            foreach (var item in filterProperties)
            {
                if (!Enum.TryParse<EmoEmotionEnum>(item.Name,out emotype))
                {
                    emotype = EmoEmotionEnum.Undetermined;
                }

                var emoEmotion = new EmoEmotion();
                emoEmotion.Score = (float)item.GetValue(scores);
                emoEmotion.EmotionType = emotype;
                emoEmotion.Face = emoface;

                emotionList.Add(emoEmotion);
            }

            return emotionList;
        }
    }
}
