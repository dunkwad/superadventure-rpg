using Engine;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public SuperAdventure()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0, 1);
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {

        }

        private void btnWest_Click(object sender, EventArgs e)
        {

        }

        private void btnEast_Click(object sender, EventArgs e)
        {

        }

        private void btnSouth_Click(object sender, EventArgs e)
        {

        }

        private void MoveTo(Location newLocation)
        {
            // Check if location requires an item
            if(newLocation.ItemRequiredToEnter != null)
            {
                bool playerHasRequiredItem = false;

                foreach(InventoryItem ii in _player.Inventory)
                {
                    if(ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        // Required Item Found
                        playerHasRequiredItem = true;
                        break; // exit out of the foreach loop
                    }
                }

                if(!playerHasRequiredItem)
                {
                    // Required Item was not found in the inventory; so display a message and stop trying to move
                    rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                    return;
                }
            }

            // Update player's current location
            _player.CurrentLocation = newLocation;

            // Display location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // Show available movement buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);

            // Heal the player & update UI
            _player.CurrentHitPoints = _player.MaximumHitPoints;
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            // Check if Location has a quest
            if(newLocation.QuestAvailableHere != null)
            {
                // Check if player already has that quest, and if they've completed it
                bool playerAlreadyHasQuest = false;
                bool playerAlreadyCompletedQuest = false;

                foreach(PlayerQuest playerQuest in _player.Quests)
                {
                    if(playerQuest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if (playerQuest.IsCompleted)
                        {
                            playerAlreadyCompletedQuest = true;
                        }
                    }
                }

                // Check if player already has the quest
                if (playerAlreadyHasQuest)
                {
                    // Check if player has not completed quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        // Check if player has required items for completion
                        bool playerHasAllItemsToCompleteQuest = true;

                        foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayersInventory = false;

                            // Check if item is in player's inventory and if there is enough quantity
                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                // Player has item in their inventory
                                if(ii.Details.ID == qci.Details.ID)
                                {
                                    foundItemInPlayersInventory = true;

                                    // Player does not have enough quantity of item for the quest
                                    if(ii.Quantity < qci.Quantity)
                                    {
                                        playerHasAllItemsToCompleteQuest = false;
                                        // No need to check other items;
                                        break;
                                    }

                                    // No need to check other items;
                                    break;
                                }
                            }

                            // If the required item wasn't found, set the flag and stop looking for items
                            if (!foundItemInPlayersInventory)
                            {
                                // Player doesn't have the item; proceed from looking for the item
                                playerHasAllItemsToCompleteQuest = false;
                                break;
                            }
                        }

                        // Player has all items required
                        if(playerHasAllItemsToCompleteQuest)
                        {
                            // Display quest completion messages
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You turn in the items for the '" + newLocation.QuestAvailableHere.Name + "' quest." + Environment.NewLine;

                            // Remove quest items from inventory
                            foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                            {
                                foreach(InventoryItem ii in _player.Inventory)
                                {
                                    if(ii.Details.ID == qci.Details.ID)
                                    {
                                        // Note: possible better way to subtract and remove from inventory
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }
                        }

                        // Give quest rewards
                        rtbMessages.Text += "You recieve: " + Environment.NewLine;
                        rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                        rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                        rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                        rtbMessages.Text += Environment.NewLine;

                        _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                        _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                        // Adding reward to player's inventory
                        bool addedItemToPlayerInventory = false;

                        // Check if player already has item in their inventory
                        foreach(InventoryItem ii in _player.Inventory)
                        {
                            // If player already has item in their inventory, increase by one
                            if(ii.Details.ID == newLocation.QuestAvailableHere.RewardItem.ID)
                            {
                                ii.Quantity++;

                                addedItemToPlayerInventory = true;
                                break;
                            }
                        }

                        // If player does not have item, add it to their inventory with quantity of 1
                        if (!addedItemToPlayerInventory)
                        {
                            _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1))
                        }

                        // Find the quest in the player's quest list, and complete it
                        foreach(PlayerQuest pq in _player.Quests)
                        {
                            if(pq.Details.ID == newLocation.QuestAvailableHere.ID)
                            {
                                pq.IsCompleted = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // Player does not have the quest

                    // Displaying quest messages
                    rtbMessages.Text += "You recieve the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with: " + Environment.NewLine;
                }
            }

        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }
    }
}
