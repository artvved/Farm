using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Game.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace Game.System
{
    public class UpdateFarmUISystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<FarmUIUpdateEventComponent> poolEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<CoinsChangedEventComponent> poolMoneyEvent = Idents.EVENT_WORLD;

        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private readonly EcsPoolInject<Coins> poolCoins = default;
        private readonly EcsPoolInject<FarmStats> poolFarmStats = default;
        private readonly EcsPoolInject<Culture> poolCulture = default;
        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
        private readonly EcsPoolInject<Tick> poolTick = default;


        private EcsFilter eventFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            eventFilter = eventWorld.Filter<FarmUIUpdateEventComponent>().End();
            playerFilter = world.Filter<PlayerTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in eventFilter)
            {
                foreach (var player in playerFilter)
                {
                    ref var coins = ref poolCoins.Value.Get(player).Value;

                    var farmEntity = poolEvent.Value.Get(entity).FarmEntity;
                    ref var farmStats = ref poolFarmStats.Value.Get(farmEntity);
                    var data = staticData.Value;
                    var screen = sceneData.Value.FarmUIScreen;
                    var farmView = (FarmView) poolView.Value.Get(farmEntity).Value;

                    if (farmStats.CurrentCulture == CultureType.NONE)
                    {
                        screen.UpdateEmpty();
                        screen.Open();
                        continue;
                    }

                    var cultureData = data.Cultures[farmStats.CurrentCulture];
                    var gsProg = data.GrowthSpeedKProgression;
                    var gscProg = data.GrowthSpeedKCostProgression;
                    var gsLvl = farmStats.GrowthSpeedLevel;

                    screen.UpdateLabel(cultureData.Icon, cultureData.Culture.CultureType);

                    SetupDefaultGroundButton(screen.defaultGroundButton, farmView);
                    SetupPicButton(screen.pictureButton, screen.size, farmView);

                    SetupButton(
                        farmEntity,
                        gsProg,
                        gscProg,
                        gsLvl, coins,
                        screen.upTimeButton,
                        UpdateGsLevel);
                    SetupButton(farmEntity,
                        data.MultKProgression,
                        data.MultKCostProgression,
                        farmStats.MultChanceLevel,
                        coins,
                        screen.upChanceButton,
                        UpdateMultLevel);

                    screen.Open();
                }
            }
        }

        private void SetupPicButton(Button pictureButton, int size, FarmView farmView)
        {
            pictureButton.onClick.RemoveAllListeners();
            pictureButton.onClick.AddListener(() => PickImage(size, farmView));
        }

        private void SetupDefaultGroundButton(Button button, FarmView farmView)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                farmView.PictureGround.gameObject.SetActive(false);
                farmView.DefaultGround.SetActive(true);
            });
        }
        

        private void PickImage(int maxSize, FarmView farmView)
        {
            if (NativeGallery.IsMediaPickerBusy())
                return;

            /*  var permissionAsync =
                  NativeGallery.RequestPermissionAsync(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);*/

            NativeGallery.GetImageFromGallery(async (path) =>
            {
                if (path != null)
                {
                    Texture2D res = null;
                    Texture2D readtexture = await NativeGallery.LoadImageAtPathAsync(path, maxSize, false);
                    if (readtexture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }

                    res = new Texture2D(readtexture.width, readtexture.height);
                    res.SetPixels(readtexture.GetPixels());
                    res.Apply();

               
                    //logic
                    var texture = res;
                    if (texture == null)
                    {
                        Debug.Log("texture is null");
                        return;
                    }

                    texture = ResizeTexture(texture, maxSize, maxSize);
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Point;
                    texture.Apply();

                    farmView.PictureGround.gameObject.SetActive(true);
                    farmView.PictureGround.material.mainTexture = texture;
                    farmView.DefaultGround.SetActive(false);
                 
                }
            });
        }

        private Texture2D ResizeTexture(Texture2D tex, int width, int height)
        {
            Texture2D resizedImage = new Texture2D(width, height);
            Texture2D bTemp = (Texture2D) GameObject.Instantiate(tex, Vector3.zero, Quaternion.identity);


            float fraction_x, fraction_y, one_minus_x, one_minus_y;
            int ceil_x, ceil_y, floor_x, floor_y;
            Color c1 = new Color();
            Color c2 = new Color();
            Color c3 = new Color();
            Color c4 = new Color();
            float red, green, blue;

            float b1, b2;

            float nXFactor = (float) bTemp.width / (float) width;
            float nYFactor = (float) bTemp.height / (float) height;

            for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
            {
                // Setup
                floor_x = (int) Mathf.Floor(((x) * nXFactor));
                floor_y = (int) Mathf.Floor(((y) * nYFactor));
                ceil_x = floor_x + 1;
                if (ceil_x >= bTemp.width) ceil_x = floor_x;

                ceil_y = floor_y + 1;
                if (ceil_y >= bTemp.height) ceil_y = floor_y;

                fraction_x = x * nXFactor - floor_x;
                fraction_y = y * nYFactor - floor_y;
                one_minus_x = 1.0f - fraction_x;
                one_minus_y = 1.0f - fraction_y;

                c1 = bTemp.GetPixel(floor_x, floor_y);
                c2 = bTemp.GetPixel(ceil_x, floor_y);
                c3 = bTemp.GetPixel(floor_x, ceil_y);
                c4 = bTemp.GetPixel(ceil_x, ceil_y);

                // Blue

                b1 = (one_minus_x * c1.b + fraction_x * c2.b);

                b2 = (one_minus_x * c3.b + fraction_x * c4.b);

                blue = (one_minus_y * (float) (b1) + fraction_y * (float) (b2));

                // Green

                b1 = (one_minus_x * c1.g + fraction_x * c2.g);

                b2 = (one_minus_x * c3.g + fraction_x * c4.g);

                green = (one_minus_y * (float) (b1) + fraction_y * (float) (b2));

                // Red

                b1 = (one_minus_x * c1.r + fraction_x * c2.r);

                b2 = (one_minus_x * c3.r + fraction_x * c4.r);

                red = (one_minus_y * (float) (b1) + fraction_y * (float) (b2));
                resizedImage.SetPixel(x, y, new Color(red, green, blue));
            }

            resizedImage.Apply();


            return resizedImage;
        }

        private void GiveMoney(int cost)
        {
            foreach (var player in playerFilter)
            {
                ref var coins = ref poolCoins.Value.Get(player);
                coins.Value -= cost;
                poolMoneyEvent.NewEntity(out int entity);
            }
        }

        private void UpdateGsLevel(int farmEnt, float k, int cost)
        {
            ref var farmStats = ref poolFarmStats.Value.Get(farmEnt);
            farmStats.GrowthSpeedLevel++;
            foreach (var cultEnt in farmStats.CultureEntities)
            {
                poolCulture.Value.Get(cultEnt).GrowthTime = k;
                poolTick.Value.Get(cultEnt).FinalTime = k;
            }

            GiveMoney(cost);
            poolEvent.NewEntity(out int ev).FarmEntity = farmEnt;
        }

        private void UpdateMultLevel(int farmEnt, float k, int cost)
        {
            ref var farmStats = ref poolFarmStats.Value.Get(farmEnt);
            farmStats.MultChanceLevel++;
            foreach (var cultEnt in farmStats.CultureEntities)
            {
                poolCulture.Value.Get(cultEnt).MultChance *= k;
            }

            GiveMoney(cost);
            poolEvent.NewEntity(out int ev).FarmEntity = farmEnt;
        }

        private void SetupButton(int farmEnt, float[] prog, int[] costProg, int level, int coins, UpButton upButton,
            Action<int, float, int> clickAction)
        {
            if (IsMax(prog, level))
                upButton.SetupButtonMax();
            else
            {
                var from = prog[level];
                var to = prog[level + 1];
                var cost = costProg[level + 1];

                upButton.SetupButton(from, to, cost, coins >= cost);
                upButton.button.onClick.RemoveAllListeners();
                upButton.button.onClick.AddListener(() => clickAction?.Invoke(farmEnt, to, cost));
            }
        }

        private bool IsMax(float[] progression, int level)
        {
            return level + 1 >= progression.Length;
        }
    }
}