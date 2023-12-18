using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Game.PlacementActionBar
{
	public struct PlacementActionButtonData
	{
		public int ItemId;
		public AssetReferenceSprite Icon;
		public int Quantity;
	}

	[RequireComponent(typeof(Toggle))]
	public class PlacementActionButton : MonoBehaviour
	{
		[SerializeField]
		public GameObject _quantityTextGo;

		[SerializeField]
		public TMP_Text _quantityText;

		[SerializeField]
		private Image _iconImage;

		[SerializeField]
		private Toggle _toggle;

		[Space(10)]
		[Header("Default Setting")]
		[SerializeField]
		private Sprite _defaultIconSprite;

		// States //
		// ====== //
		private int _currentQuantity;
		private bool _isActivatingForEdit;
		private ToggleGroup _toggleGroup;
		private IDisposable _onActivatingForEditDisposable;
		private CancellationTokenSource _cancellation;

		// Accessors //
		// ========= //
		public int ItemId { get; private set; }

		public Toggle Toggle => _toggle;

		public void UpdateRemainingCount(int inQuantity)
		{
			_currentQuantity = inQuantity;
			_quantityText.text = _currentQuantity.ToString();
		}

		public void SetupData(ToggleGroup toggleGroup, in PlacementActionButtonData data)
		{
			_toggleGroup = toggleGroup;
			toggleGroup.RegisterToggle(_toggle);
			_toggle.group = _toggleGroup;
			_currentQuantity = data.Quantity;
			ItemId = data.ItemId;
			_quantityText.text = $"x{_currentQuantity}";
			LoadIcon(data.Icon).Forget();
		}

		private async UniTask LoadIcon(AssetReferenceSprite iconReference)
		{
			_cancellation?.Cancel();
			_cancellation?.Dispose();
			_cancellation = new CancellationTokenSource();
			var spr = await iconReference.LoadAssetAsync()
				.ToUniTask(cancellationToken: _cancellation.Token);
			_iconImage.sprite = spr != null ? spr : _defaultIconSprite;
		}

		public void SetupDefault(ToggleGroup toggleGroup = null)
		{
			_toggleGroup = toggleGroup;
			_iconImage.sprite = _defaultIconSprite;
			_currentQuantity = 0;
			ItemId = -1;
			_quantityText.text = _currentQuantity.ToString();
			_onActivatingForEditDisposable?.Dispose();
			_onActivatingForEditDisposable = null;
		}
	}
}