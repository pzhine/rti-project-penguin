using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Utilities;

namespace RedRunner.Collectables
{

    public class CollectableGroup : MonoBehaviour
    {
        const int SPRITE_SCALE = 2;

        [SerializeField]
        protected List<Sprite> m_sprites;

        public void RenderChildSprites(Transform transform, List<Sprite> sprites)
        {
            Randomize.Shuffle<Sprite>(sprites);
            Queue<Sprite> spriteQ = new Queue<Sprite>(sprites);
            Debug.Log("RenderChildSprites");
            foreach(Transform child in transform)
            {
                child.localScale = new Vector3(SPRITE_SCALE, SPRITE_SCALE, SPRITE_SCALE);
                SpriteRenderer spriteRenderer = (SpriteRenderer)child.gameObject.GetComponent("SpriteRenderer");
                spriteRenderer.sprite = spriteQ.Dequeue();

            }
        }

        public void Awake()
        {
            RenderChildSprites(transform, m_sprites);
        }

    }

}