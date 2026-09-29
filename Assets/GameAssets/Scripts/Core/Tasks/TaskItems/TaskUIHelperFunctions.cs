using System.Collections.Generic;
using UnityEngine;

namespace GameTasks
{
    public static class TaskUIHelperFunctions
    {
        public static void PlaceObjects(List<RectTransform> objectsToPlace, RectTransform spawnArea)
        {
            List<RectTransform> placedObjects = new();

            foreach (RectTransform obj in objectsToPlace)
            {
                bool placed = false;

                for (int i = 0; i < 100; i++)
                {
                    Vector2 position = GetRandomPosition(obj, spawnArea);

                    position.x += 300;

                    obj.anchoredPosition = position;

                    bool overlaps = false;

                    foreach (RectTransform other in placedObjects)
                    {
                        if (IsOverlapping(obj, other))
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (!overlaps)
                    {
                        placedObjects.Add(obj);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    Debug.LogWarning($"Couldn't find space for {obj.name}");
                }
            }
        }

        private static Vector2 GetRandomPosition(RectTransform obj, RectTransform spawnArea)
        {
            Vector3[] corners = new Vector3[4];
            spawnArea.GetWorldCorners(corners);

            float minX = corners[0].x;
            float maxX = corners[2].x;

            float minY = corners[0].y;
            float maxY = corners[2].y;

            float halfWidth = obj.rect.width * obj.lossyScale.x * 0.5f;
            float halfHeight = obj.rect.height * obj.lossyScale.y * 0.5f;

            float x = Random.Range(
                minX + halfWidth,
                maxX - halfWidth
            );

            float y = Random.Range(
                minY + halfHeight,
                maxY - halfHeight
            );

            Vector3 worldPosition = new Vector3(x, y, obj.position.z);

            return spawnArea.InverseTransformPoint(worldPosition);
        }

        private static bool IsOverlapping(RectTransform a, RectTransform b)
        {
            Vector3[] aCorners = new Vector3[4];
            Vector3[] bCorners = new Vector3[4];

            a.GetWorldCorners(aCorners);
            b.GetWorldCorners(bCorners);

            Rect aRect = WorldRect(aCorners);
            Rect bRect = WorldRect(bCorners);

            return aRect.Overlaps(bRect);
        }

        private static Rect WorldRect(Vector3[] corners)
        {
            return Rect.MinMaxRect(
                corners[0].x,
                corners[0].y,
                corners[2].x,
                corners[2].y
            );
        }
    } 
}
