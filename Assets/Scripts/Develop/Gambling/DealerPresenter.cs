using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private BlackJackDealer _dealer; // Private field, will be set by SetDealer method
        
        [Tooltip("カードの3D配置を管理するコントローラー")]
        [SerializeField] private CardPlacementController _placementController;

        [Tooltip("生成するカードのプレハブ（SpriteRendererとCardViewが必須）")]
        [SerializeField] private GameObject _cardPrefab;
        
        [Tooltip("カードの移動にかかる時間")]
        [SerializeField] private float _cardDealDuration = 0.5f;

        [Tooltip("カードのスプライトを一元管理するリポジトリ")]
        [SerializeField] private CardSpriteRepository _cardSpriteRepository;

        private List<GameObject> _instantiatedCards = new List<GameObject>(); // New: Keep track of all instantiated cards


        [Header("Card Placement Settings")]
        [Tooltip("カード間の水平方向の間隔 (X軸オフセット)")]
        [SerializeField] private float _cardHorizontalOffset = 0.15f;
        [Tooltip("カード間の奥行き方向の間隔 (Z軸オフセット)。手前に来るほどマイナスの値")]
        [SerializeField] private float _cardDepthOffset = -0.01f;

        // Startはイベント購読をSetDealerに任せるため、ここでは何もしない
        private void Start() { }

        private void OnDestroy()
        {
            // オブジェクト破棄時にイベントの購読を解除
            if (_dealer != null && _dealer.Logic != null)
            {
                _dealer.Logic.OnCardDealt -= HandleCardDealt;
            }
            ClearDisplayedCards(); // Ensure all cards are destroyed on presenter destruction
        }
        
        /// <summary>
        /// 外部からBlackJackDealerインスタンスを設定し、イベント購読を開始する。
        /// </summary>
        /// <param name="dealer">設定するBlackJackDealerインスタンス</param>
        public void SetDealer(BlackJackDealer dealer)
        {
            // 既に設定されている場合は一度購読解除
            if (_dealer != null && _dealer.Logic != null)
            {
                _dealer.Logic.OnCardDealt -= HandleCardDealt;
            }

            _dealer = dealer;

            // 新しいディーラーインスタンスのイベントを購読
            if (_dealer != null && _dealer.Logic != null)
            {
                _dealer.Logic.OnCardDealt += HandleCardDealt;
            }
            else
            {
                // ここでエラーが出た場合はSetDealer呼び出し時に問題がある
                Debug.LogError("SetDealer: BlackJackDealerまたはBlackJackLogicが正しく設定されていません。", this);
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
            _instantiatedCards.Add(cardObject); // Add card to tracking list

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
            // 論理的なカード数を取得 (BlackJackLogicから)
            int logicalCardCount = 0;
            if (target == GamblingParticipant.Player)
            {
                logicalCardCount = _dealer.Logic.PlayerHand.Cards.Count;
            }
            else if (target == GamblingParticipant.Dealer)
            {
                logicalCardCount = _dealer.Logic.DealerHand.Cards.Count;
            }
            
            Vector3 targetLocalPosition = new Vector3(
                logicalCardCount * _cardHorizontalOffset,
                0,
                logicalCardCount * _cardDepthOffset
            );

            // カードを指定時間かけて目的地まで移動
            float elapsedTime = 0f;
            Vector3 startPosition = cardObject.transform.position;
            Quaternion startRotation = cardObject.transform.rotation;

            while (elapsedTime < _cardDealDuration)
            {
                // If the card was destroyed during the animation (e.g., due to game reset/bust),
                // stop the animation to prevent MissingReferenceException.
                if (cardObject == null)
                {
                    yield break; // Exit the coroutine
                }

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
            
            // Debug: ログを出力
           
            
            // スプライトの描画順を設定 (後から置かれたカードほど手前に来るように)
            cardObject.GetComponent<CardView>().SetSortingOrder(logicalCardCount);
            
            // カードを配り終えたアニメーションをトリガー（将来的な拡張箇所）
            // _dealerAnimator.SetTrigger("Idle");

            if (target == GamblingParticipant.Player)
            {
                // プレイヤーのカードは常に表向き
                cardObject.GetComponent<CardView>().Flip();
            }
            else // ディーラーの場合
            {
                // destinationParent.childCount は SetParent 後なので、
                // 今回追加されたカードが手札の「何枚目」かを示す。
                // ディーラーの2枚目以外は表向きにする
                if (logicalCardCount != 1) // 0-indexed count after current card becomes 2nd card
                {
                    cardObject.GetComponent<CardView>().Flip();
                }
                // else: ディーラーの2枚目はFlipせずに裏向きのままにする
            }
        }

        /// <summary>
        /// 表示されているすべてのカードをシーンから削除する。
        /// </summary>
        public void ClearDisplayedCards()
        {
            foreach (GameObject cardObject in _instantiatedCards)
            {
                if (cardObject != null) // Check if the object still exists before destroying
                {
                    Destroy(cardObject);
                }
            }
            _instantiatedCards.Clear(); // Clear the list after destroying all cards
        }

        /// <summary>
        /// ディーラーの裏向きのカードを表にする。
        /// </summary>
        public void RevealDealerHiddenCard()
        {
            // ディーラーの手札コンテナ内のカードを検索
            foreach (Transform child in _placementController.DealerHandContainer)
            {
                CardView cardView = child.GetComponent<CardView>();
                if (cardView != null && !cardView.IsFaceUp)
                {
                    cardView.Flip();
                    Debug.Log("ディーラーの裏向きのカードを表にしました。");
                    return; // 1枚見つけたら終了
                }
            }
            Debug.LogWarning("ディーラーの裏向きのカードが見つかりませんでした。");
        }
    }
}
