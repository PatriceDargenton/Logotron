
//  Fichier clsAfficherMsg.cs : Classes de gestion des messages via des délégués
//  -------------------------

using System;

namespace Logotron.Src.Util
{
    public sealed class clsTickEventArgs
    {
        //  Classe pour l'événement Tick : avancement d'une unité de temps : TIC-TAC
        //   utile pour mettre à jour l'heure en cours, ou pour scruter une annulation

        public clsTickEventArgs() { }
    }
    
    public sealed class clsDoEventsEventArgs
    {
        //  Classe pour l'événement WinForm.DoEvents : traiter les événements Windows
        //   après l'affichage d'un message (requis pour Windows Form)

        public clsDoEventsEventArgs() { }
    }

    public sealed class clsMsgEventArgs
    {
        //  Classe pour l'événement Message

        private string m_sMsg = "";

        public clsMsgEventArgs(string sMsg)
        {
            // If sMsg Is Nothing Then Throw New NullReferenceException
            if (sMsg == null) sMsg = "";
            this.m_sMsg = sMsg;
        }

        public string sMessage
        {
            get { return this.m_sMsg; }
        }
    }

    public sealed class clsAvancementEventArgs
    {
        //  Classe pour l'événement Avancement

        private string m_sMsg = "";

        private long m_lAvancement = 0;

        public clsAvancementEventArgs(string sMsg)
        {
            if (sMsg == null) sMsg = "";
            this.m_sMsg = sMsg;
        }

        public clsAvancementEventArgs(long lAvancement)
        {
            this.m_lAvancement = lAvancement;
        }

        public clsAvancementEventArgs(long lAvancement, string sMsg)
        {
            this.m_lAvancement = lAvancement;
            if (sMsg == null) sMsg = "";
            this.m_sMsg = sMsg;
        }

        public string sMessage
        {
            get { return this.m_sMsg; }
        }

        public long lAvancement
        {
            get { return this.m_lAvancement; }
        }
    }

    public sealed class clsSablierEventArgs
    {
        //  Classe pour l'événement Sablier

        private bool m_bDesactiver = false;

        public clsSablierEventArgs(bool bDesactiver)
        {
            this.m_bDesactiver = bDesactiver;
        }

        public bool bDesactiver
        {
            get { return this.m_bDesactiver; }
        }
    }
    public sealed class clsMsgDelegue
    {
        //  Classe de gestion des messages via des délégués

        const bool bDoEvents = true;

        public sealed class clsTickEventArgs : EventArgs { }
        public delegate void GestEvTick(object sender, clsTickEventArgs e);
        public event EventHandler<clsTickEventArgs> EvTick;

        public sealed class clsDoEventsEventArgs : EventArgs { }
        public delegate void GestEvDoEvents(object sender, clsDoEventsEventArgs e);
        public event EventHandler<clsDoEventsEventArgs> EvDoEvents;

        // CA1009 : pas réussi
        //public sealed class clsMsgEventArgs : EventArgs
        //{
        //    string sMsg { get; set; }
        //    public clsMsgEventArgs(string sMsg0)
        //    { this.sMsg = sMsg0; }
        //}
        public delegate void GestEvAfficherMessage(object sender, clsMsgEventArgs e);
        public event EventHandler<clsMsgEventArgs> EvAfficherMessage;

        //public sealed class clsAvancementEventArgs : EventArgs
        //{
        //    long lAvancement { get; set; }
        //    string sMsg { get; set; }
        //    public clsAvancementEventArgs(long lAvancement0, string sMsg0)
        //    {
        //        this.lAvancement = lAvancement0;
        //        this.sMsg = sMsg0; 
        //    }
        //}
        public delegate void GestEvAfficherAvancement(object sender, clsAvancementEventArgs e);
        public event EventHandler<clsAvancementEventArgs> EvAfficherAvancement;

        public sealed class clsSablierEventArgs : EventArgs
        {
            bool bDesactiver { get; set; }
            public clsSablierEventArgs(bool bDesactiver0)
            { this.bDesactiver = bDesactiver0; }
        } 
        public delegate void GestEvSablier(object sender, clsSablierEventArgs e);
        public event EventHandler<clsSablierEventArgs> EvSablier;

        public bool m_bAnnuler;

        public bool m_bErr;

        //  21/03/2016
        public clsMsgDelegue() { }

        public void AfficherMsg(string sMsg)
        {
            clsMsgEventArgs e = new clsMsgEventArgs(sMsg);
            EvAfficherMessage(this, e);
            if (bDoEvents) TraiterMsgSysteme_DoEvents();
        }

        void AfficherAvancement(long lAvancement, string sMsg)
        {
            clsAvancementEventArgs e = new clsAvancementEventArgs(lAvancement, sMsg);
            EvAfficherAvancement(this, e);
            if (bDoEvents) TraiterMsgSysteme_DoEvents();
        }

        void Tick()
        {
            clsTickEventArgs e = new clsTickEventArgs();
            EvTick(this, e);
            if (bDoEvents) TraiterMsgSysteme_DoEvents();
        }
        
        void DoEvents()
        {
            if (EvDoEvents == null) return;
            clsDoEventsEventArgs e = new clsDoEventsEventArgs();
            EvDoEvents(this, e);
        }

        void Sablier(bool bDesactiver = false)
        {
            clsSablierEventArgs e = new clsSablierEventArgs(bDesactiver);
            EvSablier(this, e);
            if (bDoEvents) TraiterMsgSysteme_DoEvents();
        }

        void TraiterMsgSysteme_DoEvents()
        {
            DoEvents();
        }
    }
}