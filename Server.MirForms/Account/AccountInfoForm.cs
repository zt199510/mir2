﻿using Server.MirDatabase;
using Server.MirEnvir;
using Server.MirObjects;
using System.Globalization;
using System.Security.Principal;

namespace Server
{
    public partial class AccountInfoForm : Form
    {
        private List<AccountInfo> _selectedAccountInfos;

        public Envir AccountEnvir => SMain.Envir;
        public AccountInfoForm()
        {
            InitializeComponent();

            Setup();
        }

        public AccountInfoForm(string accountId, bool match = false)
        {
            InitializeComponent();

            FilterTextBox.Text = accountId;
            MatchFilterCheckBox.Checked = match;

            Setup();
        }

        private void Setup()
        {
            RefreshInterface();
            AutoResize();

            AccountIDTextBox.MaxLength = Globals.MaxAccountIDLength;

            UserNameTextBox.MaxLength = 20;
            BirthDateTextBox.MaxLength = 10;
            QuestionTextBox.MaxLength = 30;
            AnswerTextBox.MaxLength = 30;
            EMailTextBox.MaxLength = 50;
        }

        private void AutoResize()
        {
            indexHeader.Width = -2;
            accountIDHeader.Width = -2;
            userNameHeader.Width = -2;
            bannedHeader.Width = -2;
            banReasonHeader.Width = -2;
            expiryDateHeader.Width = -2;
            Gold.Width = -2;
            GameGold.Width = -2;
        }

        private void Update(ListViewItem ListItem, AccountInfo account)
        {
            ListItem.SubItems[0].Text = account.Index.ToString();
            ListItem.SubItems[1].Text = account.AccountID;
            ListItem.SubItems[2].Text = account.UserName;
            ListItem.SubItems[3].Text = account.AdminAccount.ToString();
            ListItem.SubItems[4].Text = account.Banned.ToString();
            ListItem.SubItems[5].Text = account.BanReason;
            ListItem.SubItems[6].Text = account.ExpiryDate.ToString();
            ListItem.SubItems[7].Text = account.Gold.ToString();
            ListItem.SubItems[8].Text = account.Credit.ToString();
        }

        private ListViewItem CreateListView(AccountInfo account)
        {
            ListViewItem ListItem = new ListViewItem(account.Index.ToString()) { Tag = account };

            ListItem.SubItems.Add(account.AccountID);
            ListItem.SubItems.Add(account.UserName);
            ListItem.SubItems.Add(account.AdminAccount.ToString());
            ListItem.SubItems.Add(account.Banned.ToString());
            ListItem.SubItems.Add(account.BanReason);
            ListItem.SubItems.Add(account.ExpiryDate.ToString());
            ListItem.SubItems.Add(account.Gold.ToString());
            ListItem.SubItems.Add(account.Credit.ToString());

            return ListItem;
        }

        public void RefreshInterface()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(RefreshInterface));
                return;
            }

            List<AccountInfo> accounts = SMain.Envir.AccountList;

            long totalGold = accounts
            .Where(account => !account.AdminAccount && !account.Banned)
            .Sum(account => account.Gold);

            ServerGoldTextBox.Text = totalGold.ToString("N0", CultureInfo.GetCultureInfo("en-GB"));


            long totalCredit = accounts
            .Where(account => !account.AdminAccount && !account.Banned)
            .Sum(account => account.Credit);

            ServerCreditTextBox.Text = totalCredit.ToString("N0", CultureInfo.GetCultureInfo("en-GB"));

            if (FilterTextBox.Text.Length > 0)
                accounts = SMain.Envir.MatchAccounts(FilterTextBox.Text, MatchFilterCheckBox.Checked);

            else if (FilterPlayerTextBox.Text.Length > 0)
                accounts = SMain.Envir.MatchAccountsByPlayer(FilterPlayerTextBox.Text, MatchFilterCheckBox.Checked);

            else if (FilterIPTextBox.Text.Length > 0)
                accounts = SMain.Envir.MatchAccountsByIP(FilterIPTextBox.Text, MatchFilterCheckBox.Checked);

            if (AccountInfoListView.Items.Count != accounts.Count)
            {
                AccountInfoListView.SelectedIndexChanged -= AccountInfoListView_SelectedIndexChanged;
                AccountInfoListView.Items.Clear();
                for (int i = AccountInfoListView.Items.Count; i < accounts.Count; i++)
                {
                    AccountInfo account = accounts[i];

                    ListViewItem tempItem = CreateListView(account);

                    AccountInfoListView.Items.Add(tempItem);
                }
                AccountInfoListView.SelectedIndexChanged += AccountInfoListView_SelectedIndexChanged;
            }

            _selectedAccountInfos = new List<AccountInfo>();
            CharactersListView.Items.Clear();


            for (int i = 0; i < AccountInfoListView.SelectedItems.Count; i++)
                _selectedAccountInfos.Add(AccountInfoListView.SelectedItems[i].Tag as AccountInfo);



            if (_selectedAccountInfos.Count == 0)
            {
                AccountInfoPanel.Enabled = false;

                AccountIDTextBox.Text = string.Empty;

                UserNameTextBox.Text = string.Empty;
                BirthDateTextBox.Text = string.Empty;
                QuestionTextBox.Text = string.Empty;
                AnswerTextBox.Text = string.Empty;
                EMailTextBox.Text = string.Empty;
                return;
            }


            AccountInfo info = _selectedAccountInfos[0];

            AccountInfoPanel.Enabled = true;

            AccountIDTextBox.Enabled = _selectedAccountInfos.Count == 1;
            AccountIDTextBox.Text = info.AccountID;

            UserNameTextBox.Text = info.UserName;
            BirthDateTextBox.Text = info.BirthDate.ToShortDateString();
            QuestionTextBox.Text = info.SecretQuestion;
            AnswerTextBox.Text = info.SecretAnswer;
            EMailTextBox.Text = info.EMailAddress;

            CreationIPTextBox.Text = info.CreationIP;
            CreationDateTextBox.Text = info.CreationDate.ToString();

            LastIPTextBox.Text = info.LastIP;
            LastDateTextBox.Text = info.LastDate.ToString();

            BanReasonTextBox.Text = info.BanReason;
            BannedCheckBox.CheckState = info.Banned ? CheckState.Checked : CheckState.Unchecked;
            ExpiryDateTextBox.Text = info.ExpiryDate.ToString();
            AdminCheckBox.CheckState = info.AdminAccount ? CheckState.Checked : CheckState.Unchecked;
            PasswordChangeCheckBox.CheckState = info.RequirePasswordChange ? CheckState.Checked : CheckState.Unchecked;

            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                info = _selectedAccountInfos[i];

                if (AccountIDTextBox.Text != info.AccountID) AccountIDTextBox.Text = string.Empty;
                if (UserNameTextBox.Text != info.UserName) UserNameTextBox.Text = string.Empty;
                if (BirthDateTextBox.Text != info.BirthDate.ToShortDateString()) BirthDateTextBox.Text = string.Empty;
                if (QuestionTextBox.Text != info.SecretQuestion) QuestionTextBox.Text = string.Empty;
                if (AnswerTextBox.Text != info.SecretAnswer) AnswerTextBox.Text = string.Empty;
                if (EMailTextBox.Text != info.EMailAddress) EMailTextBox.Text = string.Empty;

                if (CreationIPTextBox.Text != info.CreationIP) CreationIPTextBox.Text = string.Empty;
                if (CreationDateTextBox.Text != info.CreationDate.ToString()) CreationDateTextBox.Text = string.Empty;


                if (LastIPTextBox.Text != info.LastIP) LastIPTextBox.Text = string.Empty;
                if (LastDateTextBox.Text != info.LastDate.ToString()) LastDateTextBox.Text = string.Empty;


                if (BanReasonTextBox.Text != info.BanReason) BanReasonTextBox.Text = string.Empty;
                if (BannedCheckBox.Checked != info.Banned) BannedCheckBox.CheckState = CheckState.Indeterminate;
                if (ExpiryDateTextBox.Text != info.ExpiryDate.ToString()) ExpiryDateTextBox.Text = string.Empty;
                if (AdminCheckBox.Checked != info.AdminAccount) AdminCheckBox.CheckState = CheckState.Indeterminate;
                if (PasswordChangeCheckBox.Checked != info.RequirePasswordChange) PasswordChangeCheckBox.CheckState = CheckState.Indeterminate;

                foreach (var character in info.Characters)
                {
                    var listItem = new ListViewItem(character.Name) { Tag = character };
                    listItem.SubItems.Add(character.Class.ToString());
                    listItem.SubItems.Add(character.Level.ToString());
                    listItem.SubItems.Add(character.PKPoints.ToString());

                    GuildObject guild = null;
                    if (character.GuildIndex != -1)
                    {
                        guild = AccountEnvir.GetGuild(character.GuildIndex);
                        if (guild != null)
                        {
                            listItem.SubItems.Add(guild.Name.ToString());
                        }
                    }
                    else
                    {
                        listItem.SubItems.Add("未加入");
                    }

                    string status = $"";

                    if (character.LastLoginDate > character.LastLogoutDate)
                    {
                        status = $"在线: {(SMain.Envir.Now - character.LastLoginDate).TotalMinutes.ToString("##")} 分钟";
                        listItem.ForeColor = Color.Green;
                    }
                    else
                    {
                        status = $"离线: {character.LastLogoutDate}";
                    }

                    if (character.Deleted)
                    {
                        status = $"注销: {character.DeleteDate}";
                        listItem.ForeColor = Color.Red;
                    }

                    listItem.SubItems.Add(status.ToString());

                    CharactersListView.Items.Add(listItem);
                }
            }
        }

        private void AccountInfoListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshInterface();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            lock (Envir.AccountLock)
            {
                SMain.Envir.CreateAccountInfo();
                RefreshInterface();
            }
        }

        private void AccountIDTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;
            if (_selectedAccountInfos.Count != 1) return;

            lock (Envir.AccountLock)
            {
                if (SMain.Envir.AccountExists(ActiveControl.Text))
                {
                    ActiveControl.BackColor = Color.Red;
                    return;
                }
                AccountInfoListView.BeginUpdate();

                ActiveControl.BackColor = SystemColors.Window;
                _selectedAccountInfos[0].AccountID = ActiveControl.Text;
                Update(AccountInfoListView.SelectedItems[0], _selectedAccountInfos[0]);

                AutoResize();
                AccountInfoListView.EndUpdate();
            }
        }

        private void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].UserName = ActiveControl.Text;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void BirthDateTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            DateTime temp;

            if (!DateTime.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;

            for (int i = 0; i < _selectedAccountInfos.Count; i++)
                _selectedAccountInfos[i].BirthDate = temp;
        }

        private void QuestionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedAccountInfos.Count; i++)
                _selectedAccountInfos[i].SecretQuestion = ActiveControl.Text;
        }

        private void AnswerTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedAccountInfos.Count; i++)
                _selectedAccountInfos[i].SecretAnswer = ActiveControl.Text;
        }

        private void EMailTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;


            for (int i = 0; i < _selectedAccountInfos.Count; i++)
                _selectedAccountInfos[i].EMailAddress = ActiveControl.Text;
        }

        private void DayBanButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否禁用选定账户", "禁用账户", MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;

            DateTime expiry = SMain.Envir.Now.AddDays(1);

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].Banned = true;
                _selectedAccountInfos[i].ExpiryDate = expiry;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            RefreshInterface();
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void WeekBanButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否禁用选定账户", "禁用账户", MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;

            DateTime expiry = SMain.Envir.Now.AddDays(7);

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].Banned = true;
                _selectedAccountInfos[i].ExpiryDate = expiry;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            RefreshInterface();
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void PermBanButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否禁用选定账户", "禁用账户", MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;


            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].Banned = true;
                _selectedAccountInfos[i].ExpiryDate = DateTime.MaxValue;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            RefreshInterface();
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void BannedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].Banned = false;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void BanReasonTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].BanReason = ActiveControl.Text;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void ExpiryDateTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            DateTime temp;

            if (!DateTime.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].ExpiryDate = temp;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshInterface();
        }

        private void AccountInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SMain.Envir.Running) return;

            SMain.Envir.SaveAccounts();
        }

        private void AdminCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].AdminAccount = AdminCheckBox.CheckState == CheckState.Checked ? true : false;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void WipeCharButton_Click(object sender, EventArgs e)
        {
            if (SMain.Envir.Running)
            {
                MessageBox.Show("服务器运行中不能删除账户数据", "通知",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if (MessageBox.Show("是否删除数据库中所有账户数据", "通知",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                for (int i = 0; i < SMain.Envir.AccountList.Count; i++)
                {
                    AccountInfo account = SMain.Envir.AccountList[i];

                    account.Characters.Clear();
                }

                SMain.Envir.Auctions.Clear();
                SMain.Envir.GuildList.Clear();

                MessageBox.Show("所有账户相关数据已清除", "通知", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangePasswordDialog PasswordDialog = new ChangePasswordDialog();

            if (PasswordDialog.ShowDialog(this) == DialogResult.OK && PasswordDialog.PasswordTextBox.Text.Length > 0)
            {
                AccountInfoListView.BeginUpdate();
                for (int i = 0; i < _selectedAccountInfos.Count; i++)
                {
                    _selectedAccountInfos[i].Password = PasswordDialog.PasswordTextBox.Text;
                    _selectedAccountInfos[i].RequirePasswordChange = true;
                    PasswordChangeCheckBox.CheckState = CheckState.Checked;
                    Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
                    MessageBox.Show("密码已变更");
                }

                AutoResize();
                AccountInfoListView.EndUpdate();
            }
        }

        private void PasswordChangeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].RequirePasswordChange = PasswordChangeCheckBox.CheckState == CheckState.Checked;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        #region IPSearch
        private void CreationIPSearch_Click(object sender, EventArgs e)
        {
            string ipAddress = CreationIPTextBox.Text;

            string url = $"https://127.0.0.1/ip/{ipAddress}";//默认 whatismyipaddress.com

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开网址时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LastIPSearch_Click(object sender, EventArgs e)
        {
            string ipAddress = LastIPTextBox.Text;

            string url = $"https://127.0.0.1/ip/{ipAddress}";//默认 whatismyipaddress.com

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开网址时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void Delaccbtn_Click(object sender, EventArgs e)
        {
            if (AccountInfoListView.SelectedItems.Count > 0)
            {
                // Ask for confirmation
                DialogResult result = MessageBox.Show("您确定要删除此帐户吗？", "永久删除账户确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Get the selected account from the ListViewItem's Tag property
                    AccountInfo accInfo = (AccountInfo)AccountInfoListView.SelectedItems[0].Tag;

                    // Remove the selected account from AccountList
                    if (SMain.Envir.AccountList.Contains(accInfo))
                    {
                        SMain.Envir.AccountList.Remove(accInfo);
                    }

                    // Remove the selected item from AccountInfoListView
                    AccountInfoListView.SelectedItems[0].Remove();
                }
            }
            else
            {
                MessageBox.Show("请选择要删除的帐户");
            }
        }
    }
}
