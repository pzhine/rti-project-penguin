using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Utilities;

namespace RedRunner.Collectables
{

    public class CollectableGroup : MonoBehaviour
    {
        const int SPRITE_HEIGHT = 300;

        [SerializeField]
        protected List<Sprite> m_sprites;

        public void RenderChildSprites(Transform transform, List<Sprite> sprites)
        {
            Randomize.Shuffle<Sprite>(sprites);
            Queue<Sprite> spriteQ = new Queue<Sprite>(sprites);
            Debug.Log("RenderChildSprites");
            float xScale, yScale, aspect;
            SpriteRenderer spriteRenderer;
            foreach(Transform child in transform)
            {
                // calculate the scale
                spriteRenderer = (SpriteRenderer)child.gameObject.GetComponent("SpriteRenderer");
                spriteRenderer.sprite = spriteQ.Dequeue();
                aspect = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.rect.height;
                yScale = SPRITE_HEIGHT / spriteRenderer.sprite.rect.height;
                xScale = yScale * aspect;

                child.localScale = new Vector3(xScale, yScale);
                //Debug.Log("width: " + spriteRenderer.sprite.rect.width);
                //Debug.Log("height: " + spriteRenderer.sprite.rect.height);
                //Debug.Log("scale: " + yScale);
            }
        }

        public void Awake()
        {
            RenderChildSprites(transform, m_sprites);
        }

    }

}