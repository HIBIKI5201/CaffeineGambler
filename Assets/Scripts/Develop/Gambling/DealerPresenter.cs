using System.Collections;
using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックのディーラー演出を担当するプレゼンター。
    /// ロジック層からのイベントを受け取り、3Dモデルの挙動やカードの移動を制御する。
    /// </summary>
    public class DealerPresenter : MonoBehaviour
    {
        [Tooltip("ロジック層への参照を持つディーラー本体")]
        [SerializeField] private BlackJackDealer _dealer;
        
        [Tooltip("カードの3D配置を管理するコントローラー")]
        [SerializeField] private CardPlacementController _placementController;

        [Tooltip("生成するカードのプレハブ（SpriteRendererとCardViewが必須）")]
        [SerializeField] private GameObject _cardPrefab;
        
        [Tooltip("カードの移動にかかる時間")]
        [SerializeField] private float _cardDealDuration = 0.5f;

        [Tooltip("カードのスプライトを一元管理するリポジトリ")]
        [SerializeField] private CardSpriteRepository _cardSpriteRepository;

        private void Start()
        {
            // ゲームロジックのイベントを購読
            if (_dealer != null && _dealer.Logic != null)
            {
                _dealer.Logic.OnCardDealt += HandleCardDealt;
            }
            else
            {
                Debug.LogError("BlackJackDealerまたはBlackJackLogicが設定されていません。", this);
            }
        }

        private void OnDestroy()
        {
            // オブジェクト破棄時にイベントの購読を解除
            if (_dealer != null && _dealer.Logic != null)
            {
                _dealer.Logic.OnCardDealt -= HandleCardDealt;
            }
        }
        
        /// <summary>
        /// カードが配られた際のイベントハンドラ。
        /// </summary>
        /// <param name="participant">カードが配られた対象</param>
        /// <param name="card">配られたカードのデータ</param>
        private void HandleCardDealt(GamblingParticipant participant, Card card)
        {
            // 1. リポジトリからスプライトを取得
            Sprite frontSprite = _cardSpriteRepository.GetSprite(card);
            Sprite backSprite = _cardSpriteRepository.CardBackSprite;
            if (frontSprite == null || backSprite == null)
            {
                Debug.LogError($"カード({card})のスプライト設定が不十分です。", this);
                return;
            }
            
            // 2. カードをディーラーの手元に生成
            if (_cardPrefab == null)
            {
                Debug.LogError("カードのプレハブが設定されていません。", this);
                return;
            }
            GameObject cardObject = Instantiate(_cardPrefab, _placementController.DealerCardOrigin.position, _placementController.DealerCardOrigin.rotation);

            // 3. CardViewにスプライトと値を設定
            CardView cardView = cardObject.GetComponent<CardView>();
            if (cardView == null)
            {
                Debug.LogError("カードプレハブにCardViewコンポーネントがありません。", cardObject);
                Destroy(cardObject);
                return;
            }
            cardView.Initialize(card, frontSprite, backSprite);

            // 4. アニメーションを開始
            StartCoroutine(DealCardAnimation(cardObject, participant));
        }

        /// <summary>
        /// カードを配る一連の演出を行うコルーチン。
        /// </summary>
        /// <param name="cardObject">演出対象のカード</param>
        /// <param name="target">配る対象</param>
        private IEnumerator DealCardAnimation(GameObject cardObject, GamblingParticipant target)
        {
            // ディーラーのアニメーションをトリガー（将来的な拡張箇所）
            // _dealerAnimator.SetTrigger("Deal");

            // 移動先の決定
            Transform destinationParent = target == GamblingParticipant.Player 
                ? _placementController.PlayerHandContainer 
                : _placementController.DealerHandContainer;
            
            // 手札コンテナ内のカード数に基づいて、新しいカードのローカル位置を決定
            //（ここでは単純に横に並べる例）
            // ※カードが追加される前にDestinationが確定するので、childCountは現在の枚数を示す
            Vector3 targetLocalPosition = new Vector3(destinationParent.childCount * 0.15f, 0, 0);

            // カードを指定時間かけて目的地まで移動
            float elapsedTime = 0f;
            Vector3 startPosition = cardObject.transform.position;
            Quaternion startRotation = cardObject.transform.rotation;

            while (elapsedTime < _cardDealDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _cardDealDuration);

                cardObject.transform.position = Vector3.Lerp(startPosition, destinationParent.TransformPoint(targetLocalPosition), t);
                cardObject.transform.rotation = Quaternion.Slerp(startRotation, destinationParent.rotation, t);
                
                yield return null;
            }

            // 移動完了後、正式に手札コンテナの子要素にする
            cardObject.transform.SetParent(destinationParent, worldPositionStays: true);
            cardObject.transform.localPosition = targetLocalPosition;
            cardObject.transform.localRotation = Quaternion.identity;
            
            // カードを配り終えたアニメーションをトリガー（将来的な拡張箇所）
            // _dealerAnimator.SetTrigger("Idle");

            // 例：ディーラーの2枚目のカードは伏せておく、などのロジックをここに追加できる
            // if(target == GamblingParticipant.Dealer && destinationParent.childCount == 2)
            // {
            //     // そのまま（裏面のまま）
            // }
            // else
            // {
            //     cardObject.GetComponent<CardView>().Flip();
            // }
        }
    }
}
