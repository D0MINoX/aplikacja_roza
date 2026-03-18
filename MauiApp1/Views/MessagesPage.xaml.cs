using MauiApp1.Models;

namespace MauiApp1.Views;

public partial class MessagesPage : ContentPage
{
	public MessagesPage()
	{
		InitializeComponent();
        var testMessages = new List<RosaryMessage>
        {
            new RosaryMessage
            {
                MessageTitle = "Spotkanie Róży",
                MessageBody = "Przypominam o jutrzejszym spotkaniu w salce o godzinie 19:00. Obecność obowiązkowa!",
                AuthorName = "Zelator Jan",
                CreatedAt = DateTime.Now
            },
            new RosaryMessage
            {
                MessageTitle = "Zmiana tajemnic",
                MessageBody = "Od niedzieli przechodzimy na tajemnice radosne. Proszę sprawdzić swoje przypisania w zakładce 'Moja Róża'.",
                AuthorName = "Admin",
                CreatedAt = DateTime.Now.AddHours(-5)
            },
            new RosaryMessage
            {
                MessageTitle = "Intencja na marzec",
                MessageBody = "W tym miesiącu modlimy się szczególnie za osoby chore i cierpiące w naszej parafii.",
                AuthorName = "Ks. Proboszcz",
                CreatedAt = DateTime.Now.AddDays(-2)
            },
            new RosaryMessage
            {
                MessageTitle = "Dłuższe ogłoszenie organizacyjne",
                MessageBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin nibh augue, suscipit a, scelerisque sed, lacinia in, mi. Cras vel lorem. Etiam pellentesque aliquet tellus. Phasellus pharetra nulla ac diam. Quisque semper justo at risus. Donec venenatis, turpis vel hendrerit interdum, dui ligula ultricies purus, sed posuere libero dui id orci.",
                AuthorName = "Sekretariat",
                CreatedAt = DateTime.Now.AddDays(-3)
            }
            
        };

        // Przypisanie do CollectionView
        MessagesList.ItemsSource = testMessages;
    }
}