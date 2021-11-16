public interface IOrderHandler
{
    void OnOfferTriggerEnter(BDeliveryOrder order);
    void OnOfferTriggerExit(BDeliveryOrder order);
}