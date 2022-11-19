' Copyright 2020 FUJITSU Limited
' ****************************************************************************************************
' システム名     ： [新ＭＪＰＣリビルド]
' プログラムID   ： [QC001F04]
' プログラム名   ： [見積・契約入力【たよ明細タブ】]
' 新規作成       ： [東和)于 世翔]
' 新規作成日     ： [2020/08/17]
' ****************************************************************************************************
' 改版履歴
' ****************************************************************************************************
' バージョン             名前                    日付
' (変更内容)
' (1)V1.0.0              [東和)于 世翔]         [2020/08/17]
' 新規作成
' ****************************************************************************************************
Imports FarPoint.Win.Spread
Imports Jp.Co.OtsukaShokai.MJPC.BusinessCommon.Common.Const
Imports Jp.Co.OtsukaShokai.MJPC.Common.Base.Util
Imports Jp.Co.OtsukaShokai.MJPC.Common.Base.Const
Imports Jp.Co.OtsukaShokai.MJPC.Client.Base.Util
Imports System.ComponentModel
Imports Jp.Co.OtsukaShokai.MJPC.Common.Base.Dto
Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.Globalization
Imports Jp.Co.OtsukaShokai.MJPC.Dto
Imports Jp.Co.OtsukaShokai.MJPC.Common.Client.Util
Imports Jp.Co.OtsukaShokai.MJPC.Client.QC001.QC001F00
Imports Jp.Co.OtsukaShokai.MJPC.Common.Base.Scope
Imports Jp.Co.OtsukaShokai.MJPC.BusinessCommon.QC001.QC001F00
Imports Jp.Co.OtsukaShokai.MJPC.BusinessCommon.Common.Util

Namespace Client.QC001.QC001F04
    ''' <summary>
    ''' 見積・契約入力
    ''' </summary>
    Public Class QC001F04Form
        Inherits BaseBusinessForm
        ''' <summary>
        ''' フォームID
        ''' </summary>
        Public Shared ReadOnly FORM_ID As String = "QC001F04"
        ''' <summary>
        ''' フォーム名
        ''' </summary>
        Public Shared ReadOnly FORM_NAME As String = "見積・契約入力【たよ明細タブ】"

        '#12314 2022/08/15 OEC)Fujiwara ADD start
        Public Shared ReadOnly MAX_DATE As DateTime = DateTime.ParseExact("2099/12/31", "yyyy/MM/dd", Nothing)
        '#12314 2022/08/15 OEC)Fujiwara ADD end

        ''' <summary>
        ''' フォームデータ保持
        ''' </summary>
        Public qc001F04FormDto As New QC001F04FormDto
        ''' <summary>
        ''' 1ページあたりの表示件数退避用
        ''' </summary>
        Public oldPerPageSize As String
        ''' <summary>
        ''' 現在のページ数退避用
        ''' </summary>
        Public oldCurrentPage As String
        ''' <summary>
        ''' 変更フラグ
        ''' </summary>
        Private changedFlg As Boolean = False
        ''' <summary>
        ''' セル変更フラグ
        ''' </summary>
        Private cellChangeFlg As Boolean = True
        ''' <summary>
        ''' セル変更フラグ
        ''' </summary>
        Private cellChangeFlg2 As Boolean = True
        ''' <summary>
        ''' SPREAD上でのShift押下中フラグ
        ''' </summary>
        Private sprM1ShiftFlg As Boolean = False
        ''' <summary>
        ''' SPREAD上でのCtrl押下中フラグ
        ''' </summary>
        Private sprM1CtrlFlg As Boolean = False
        ''' <summary>
        ''' SPREADの選択行リスト
        ''' </summary>
        Private allSelectRowList As New List(Of Integer)
        '#12150 2022.08.08 START
        ''' <summary>
        ''' 現在のページ数変更フラグ
        ''' </summary>
        Private bFlag As Boolean = False
        '#12150 2022.08.08 END
        '#11292 ADD start
        ''' <summary>
        ''' たよ表示状態
        ''' </summary>
        Private tayoDisptype As String = Consts.TayoDisptype.Normal
        ''' <summary>
        ''' 物販表示状態
        ''' </summary>
        Private bupDisptype As String = Consts.BupDisptype.Normal
        '#11292 ADD end

        '#12125 20220921 ADD-START 
        Public isLoadBreak As Boolean = False
        '#12125 20220921 ADD-End

#Region "QC001F04Form.vb 内部＿変数"
        ''' <summary>
        ''' 設定値トラン
        ''' </summary>

        Private _settingDataForGamenIdList As New List(Of SettingDto)

#End Region

        ''' <summary>
        ''' M1＿明細定数
        ''' </summary>
        Public Enum buppanEnum
            txtM1No
            txtM1MenuNo
            txtM1MenuNm
            txtM1Syubetu
            lblM1KeiyakuTani
            lblM1Seikyu
            lblM1Futai
            lblM1AddonHissu
            lblM1AddonSuisho
            lblM1ItakuKibo
            txtM1Sryo
            lblM1NengakuTeika
            txtM1NengakuNebikiPar
            lblM1NengakuBinTnk
            lblM1NengakuHiyo
            lblM1GetsugakuTeika
            txtM1GetsugakuNebikiPar
            lblM1GetsugakuBinTnk
            lblM1GetsugakuHiyo
            lblM1MusyoShokiHiyo
            lblM1ShokiHiyo
            lblM1MusyoZuijiHiyo
            lblM1ZuijiHiyo
            lblM1GnkKbn
            txtM1HyojunGnk
            txtM1AtoArari
            lblM1ArariPar
            lblM1GetsugakuMusyoMoNum
            cmbM1SettisakiCombo
            cmbM1GroupCombo
            M1SubTtl
            lblM1SubTtl
        End Enum

        ''' <summary>
        ''' M2＿物販明細タブ＿商品明細合計定数
        ''' </summary>
        Public Enum buppanGokeiEnum
            sprM2GokeiShbt
            sprM2GokeiShbt2
            lblM2GokeiranNengakuTeika
            lblM2GokeiranNengakuNebikigaku
            lblM2GokeiranNengakuHiyo
            lblM2GokeiranGetsugakuTeika
            lblM2GokeiranGetsugakuNebikigaku
            lblM2GokeiranGetsugakuHiyo
            lblM2GokeiranShokiHiyo
            lblM2GokeiranZuijiHiyo
            lblM2GokeiranHyojunGnk
            lblM2GokeiranArarigaku
            lblM2GokeiranArariPar
            lblM2GetsugakuKansangoranGetsugakuHiyo
            lblM2GetsugakuKansangoranHyojunGnk
            lblM2GetsugakuKansangoranArarigaku
            lblM2GetsugakuKansangoranArariPar
        End Enum
        ''' <summary>
        ''' コンストラクタです。
        ''' </summary>
        Public Sub New()
            InitializeComponent()
            MyBase.FormId = FORM_ID
            MyBase.formName = FORM_NAME

            Me.DoubleBuffered = True
            Me.mainForm = mainForm
            '12814　6472 横展開 begin
            Me.lblGokeiHyojiSettei.MaximumSize = New System.Drawing.Size(97, 22)
            Me.rdoGokeiHyojiSetteiKakinRadio.MaximumSize = New System.Drawing.Size(54, 22)
            Me.rdoGokeiHyojiSetteiHoshuRadio.MaximumSize = New System.Drawing.Size(54, 22)
            Me.btnMeisaiSoGokei.MaximumSize = New System.Drawing.Size(108, 22)
            Me.txtIchiPageNoKensuu.MaximumSize = New System.Drawing.Size(43, 24)
            Me.lblGaitouKensuu.MaximumSize = New System.Drawing.Size(47, 22)
            Me.btnRowInsert.MaximumSize = New System.Drawing.Size(95, 25)
            Me.btnPaste.MaximumSize = New System.Drawing.Size(95, 25)
            Me.btnCut.MaximumSize = New System.Drawing.Size(95, 25)
            Me.btnCopy.MaximumSize = New System.Drawing.Size(95, 25)
            Me.sprHoshuRyokinSansyutsuKijunDate.MaximumSize = New System.Drawing.Size(98, 24)
            Me.Panel1.MaximumSize = New System.Drawing.Size(30, 149)
            Me.btnNarabikaeUp.MaximumSize = New System.Drawing.Size(20, 22)
            Me.btnNarabikaeDown.MaximumSize = New System.Drawing.Size(20, 22)
            Me.lblNarabikae.MaximumSize = New System.Drawing.Size(23, 75)
            Me.Label1.MaximumSize = New System.Drawing.Size(14, 18)
            Me.Panel15.MaximumSize = New System.Drawing.Size(101, 44)
            Me.rdoNebikiSetteiMenuBetsuRadio.MaximumSize = New System.Drawing.Size(80, 22)
            Me.rdoNebikiSetteiZidoAnbunRadio.MaximumSize = New System.Drawing.Size(82, 22)
            Me.btnHabaKioku.MaximumSize = New System.Drawing.Size(108, 22)
            Me.btnAllHyojiSetteiHaba.MaximumSize = New System.Drawing.Size(108, 22)
            Me.lblMeisaiHyojiSetteiSettisaki.MaximumSize = New System.Drawing.Size(52, 22)
            Me.txtHoshuryoCmt.MaximumSize = New System.Drawing.Size(271, 24)
            Me.txtKeiyakuShikibetsu2.MaximumSize = New System.Drawing.Size(94, 24)
            Me.lblHoshuryoCmt.MaximumSize = New System.Drawing.Size(98, 22)
            '2022/09/02 MOD-START #13101　「保守料金算出基準日」となっていない
            'Me.lblHoshuRyokinSansyutsuKijunDate.MaximumSize = New System.Drawing.Size(135, 22)
            Me.lblHoshuRyokinSansyutsuKijunDate.MaximumSize = New System.Drawing.Size(150, 22)
            '2022/09/02 MOD-END #13101　「保守料金算出基準日」となっていない
            Me.cmbHoshuKbnCombo.MaximumSize = New System.Drawing.Size(186, 25)
            Me.lblHoshuKbn.MaximumSize = New System.Drawing.Size(72, 22)
            Me.btnSaigouHyouji.MaximumSize = New System.Drawing.Size(42, 22)
            Me.btnTsugiPageHyouji.MaximumSize = New System.Drawing.Size(26, 22)
            Me.btnSentouHyouji.MaximumSize = New System.Drawing.Size(33, 22)
            Me.btnMaePegeHyouji.MaximumSize = New System.Drawing.Size(26, 22)
            Me.lblGaitouKensuu.MaximumSize = New System.Drawing.Size(47, 22)
            Me.C1Label193.MaximumSize = New System.Drawing.Size(89, 22)
            Me.lblSouPageSuu.MaximumSize = New System.Drawing.Size(47, 22)
            Me.C1Label202.MaximumSize = New System.Drawing.Size(81, 22)
            Me.txtIchiPageNoKensuu.MaximumSize = New System.Drawing.Size(43, 24)
            Me.txtGenzaiNoPage.MaximumSize = New System.Drawing.Size(43, 24)
            Me.cmbNebikiSetteiMarumeSetteiCombo.MaximumSize = New System.Drawing.Size(89, 25)
            Me.cmbMeisaiHyojiSetteiSettisakiCombo.MaximumSize = New System.Drawing.Size(170, 25)
            Me.btnSeigoCheck.MaximumSize = New System.Drawing.Size(108, 22)
            Me.btnRyokinSaiKeisan.MaximumSize = New System.Drawing.Size(108, 22)
            Me.btnFutaiNyuryoku.MaximumSize = New System.Drawing.Size(108, 22)
            Me.btnTaKyoten.MaximumSize = New System.Drawing.Size(108, 22)
            Me.btnMenuFutai.MaximumSize = New System.Drawing.Size(108, 22)
            Me.btnSuishoKosei.MaximumSize = New System.Drawing.Size(108, 22)
            Me.btnMenuSentaku.MaximumSize = New System.Drawing.Size(108, 22)
            Me.lblKeiyakuShikibetsu2.MaximumSize = New System.Drawing.Size(81, 22)
            Me.lblMeisaiHyoji.MaximumSize = New System.Drawing.Size(98, 22)
            Me.lblMarumeSettei.MaximumSize = New System.Drawing.Size(81, 22)
            Me.lblNebikiSettei.MaximumSize = New System.Drawing.Size(98, 22)
            Me.lblNebikiSettei2.MaximumSize = New System.Drawing.Size(98, 22)
            Me.C1Label158.MaximumSize = New System.Drawing.Size(71, 22)
            Me.btnMeisaiSansyo.MaximumSize = New System.Drawing.Size(108, 22)
            Me.btnGroupHenko.MaximumSize = New System.Drawing.Size(108, 22)
            '12814　6472 横展開 end
        End Sub

#Region "エントリー"

        ''' <summary>
        ''' 初期表示
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Sub QC001F04Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            '開始処理を行う
            InitProcess()

            ClientLogUtil.Logger.DebugAP("QC001F04Form:QC001F04Form_Load start")

            ' ### ADD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
            ' ### イベントハンドル停止をForm_Load直後に実施する
            'イベントハンドルを一時停止
            Me.StopHandler()
            ' ### ADD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            '共通領域のクライアントログの出力
            'SharedComClient.InstanceData.QC001_DebugLog(Me.FORM_NAME, "初期表示", "")

            '#12314 2022/08/15 OEC)Fujiwara ADD start
            sprHoshuRyokinSansyutsuKijunDate.MaxDate = MAX_DATE
            '#12314 2022/08/15 OEC)Fujiwara ADD end

            '#13115 20220901 ADD START 引用時、タブ順ためチェックを追加
            If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.InyoAdd) Then
                Me.bFlag = True
            End If
            '#13115 20220901 ADD END 引用時、タブ順ためチェックを追加

            'フォームＤＴＯ取得／生成
            Me.qc001F04FormDto = If(SharedComClient.InstanceData.QC001F04FormDTO, New QC001F04FormDto)

            '#6573、#8043、#8406 追加 START
            Dim settingUtilInit As New SettingUtil(FormId, FormId, Guid.NewGuid().ToString)
            Dim SelectsettingDtoList As New List(Of SettingDto)
            Dim UpdatesettingDtoList As New List(Of SettingDto)
            '性能横展開対応_設定値トラン取得（ForGamenId）
            'SelectsettingDtoList = settingUtilInit.GetSettingDataForGamenId(FormId, BusinessConst.ZZZZZ)
            Dim reqSettingDtoList As New List(Of SettingDto)
            '１頁表示件数
            Dim sDtodispResltNum As New SettingDto
            ' ### UPD-START KATO 2022/09/02 クリアボタンクリック時の不具合対応
            ' ### ItemIdに設定するフォームIDを固定値に変更
            sDtodispResltNum.ItemId = FORM_ID
            ' sDtodispResltNum.ItemId = FormId
            ' ### UPD-END KATO 2022/09/02 クリアボタンクリック時の不具合対応
            sDtodispResltNum.Section = "１頁の件数"
            sDtodispResltNum.Param = "商品明細"
            sDtodispResltNum.SettingFileName = "NA"
            reqSettingDtoList.Add(sDtodispResltNum)

            '設定値トラン取得部品呼出し
            settingUtilInit.GetSettingDataList(reqSettingDtoList, _settingDataForGamenIdList)
            SelectsettingDtoList.AddRange(_settingDataForGamenIdList)

            '共通部品クラスを呼び出し、１頁表示件数を取得
            Dim dispResltNum = settingUtilInit.GetValueForSettingDto(SelectsettingDtoList, "１頁の件数", "商品明細", "NA")

            '初回未設定の場合、１頁表示件数初期表示値は「C_DataCntPerPage」となります。
            If dispResltNum Is Nothing Then
                '設定値トランDTOリスト
                ' ### UPD-START KATO 2022/09/02 クリアボタンクリック時の不具合対応
                ' ### ItemIdに設定するフォームIDを固定値に変更
                Dim settingInitDto = New SettingDto With {
                    .ItemDivision = "G",
                    .ItemId = FORM_ID，
                    .Remarks = String.Empty,
                    .Section = "１頁の件数",
                    .SettingFileName = "NA"，
                    .TenkaCode = "ZZZZZ",
                    .UserId = ApplicationScope.LoginInfo.SyainCode,
                    .Param = "商品明細"
                }

                ' Dim settingInitDto = New SettingDto With {
                ' .ItemDivision = "G",
                ' .ItemId = FormId，
                ' .Remarks = String.Empty,
                ' .Section = "１頁の件数",
                ' .SettingFileName = "NA"，
                ' .TenkaCode = "ZZZZZ",
                ' .UserId = ApplicationScope.LoginInfo.SyainCode,
                ' .Param = "商品明細"
                ' }

                ' ### UPD-END KATO 2022/09/02 クリアボタンクリック時の不具合対応

                settingInitDto.Value = Consts.PerPageSize_QC001F04
                settingInitDto.UpdateFlag = True
                UpdatesettingDtoList.Add(settingInitDto)

                '(3)-3.共通部品呼び出し
                settingUtilInit.UpdateSettingData(UpdatesettingDtoList)

                qc001F04FormDto.PerPageSize = Consts.PerPageSize_QC001F04
                Me.txtIchiPageNoKensuu.Text = Consts.PerPageSize_QC001F04
            Else
                ' 2022/10/24 張Inc)金　#9270 UPD START
                Me.txtIchiPageNoKensuu.Text = dispResltNum.ToString
                qc001F04FormDto.PerPageSize = dispResltNum.ToString
                ' 2022/09/27 張Inc)金　#9270 UPD START
                'Me.txtIchiPageNoKensuu.Text = Consts.PerPageSize_QC001F04
                'qc001F04FormDto.PerPageSize = Consts.PerPageSize_QC001F04
                ' 2022/09/27 張Inc)金　#9270 UPD END
                ' 2022/10/24 張Inc)金　#9270 UPD END
            End If

            ' ### ADD-START KATO 2022/08/31 性能改善（パーソナル設定取得サービスの呼出し回数削減）
            ' ### GetPersonalで取得していた明細上限件数を本メソッド内で取得しサービス呼出し回数を削減

            ' パーソナル設定（明細上限件数）の取得
            Dim mesaiJyougen As Integer =
            CInt(settingUtilInit.GetValueForSettingDto(settingUtilInit.GetDefaultSettingDataForGamenId("QC001F04"), "たよ明細タブ", "明細上限件数", "NA"))
            If mesaiJyougen > 0 Then
                qc001F04FormDto.hdnMesaiJyougen = mesaiJyougen
            End If

            ' ### ADD-End KATO 2022/08/31 性能改善（パーソナル設定取得サービスの呼出し回数削減）

            ' 2021/08/12 #4813
            'If String.IsNullOrWhiteSpace(qc001F04FormDto.PerPageSize) Then
            'qc001F04FormDto.PerPageSize = Consts.PerPageSize_QC001F04
            ''2022.07.04 #ST 11057 ADD-START 1ページあたりの件数がタブ移動した後戻ってしまう修正
            'Else
            'Me.txtIchiPageNoKensuu.Text = qc001F04FormDto.PerPageSize
            'End If
            '2022.07.04 #ST 11057 ADD-END 1ページあたりの件数がタブ移動した後戻ってしまう修正
            '#6573、#8043、#8406 追加 END
            SharedComClient.InstanceData.QC001F04FormDTO = Me.qc001F04FormDto

            Me.changedFlg = False

            ' ### DEL-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
            ' ### イベントハンドル停止はForm_Load直後に実施する
            'イベントハンドルを一時停止
            'Me.StopHandler()
            ' ### DEL-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            ' フォームロードフラグを立てる（画面表示時にAction側で不要な処理を飛ばす為のフラグ）
            qc001F04FormDto.FormLoadFlg = True
            '#9084 GOMADA 2022/09/08 UPDATE START
            '推奨構成保守区分チェックフラグを立てる
            qc001F04FormDto.msuishoHoshkbnCheckFlg = True
            '#9084 GOMADA 2022/09/08 UPDATE END
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "QC001F04Form_Load"
            UpdateProcessingFlagToFalse()
            qc001F04FormDto = DirectCast(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            '#12125 20220921 ADD-START
            Dim qc001F01FormDto = SharedComClient.InstanceData.QC001F01FormDTO
            '得意先コードが空白の場合、メッセージを表示する。
            If String.IsNullOrEmpty(qc001F01FormDto.TxtTokuisakiCd) Then
                If Not qc001F04FormDto.ContinueFlg Then
                    Dim qc001f00Form = CType(Me.Parent.Parent, QC001F00Form)
                    qc001f00Form.SwitchTab("QC001F01")
                    Me.isLoadBreak = True
                    ClientLogUtil.Logger.DebugAP("QC001F04Form:QC001F04_Load Return1")
                    Return
                End If
            End If
            '#12125 20220921 ADD-End

            ' フォームロードフラグの初期化
            qc001F04FormDto.FormLoadFlg = False

            '明細幅設定
            Me.MeisaiHabaSetting(CommUtility.ConvertSettingDto(qc001F04FormDto.HyojiHabaList))

            ' 画面復元
            Me.returnheda()

            ' 画面項目編集
            Me.EditGamenKomoku()

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()

            ' 画面表示
            Me.Paging()
            qc001F04FormDto.ContinueFlg = True

            ' 明細のキー操作制御
            setInputMapkeys(sprM1MenuIchiran)
            setInputMapkeys(sprM2GokeiIchiran)

            '#12691 2022/08/23 START
            ' QC001F00Formのアドオンチェックボタン制御
            'Me.CallQC001F00FormAddonCheckSeigyo()
            Me.CallQC001F00FormAddonCheckSeigyo(QC001F00Form.STATUS_99)
            '#12691 2022/08/23 END

            ' ### DEL-START KATO 2022/08/31 性能改善（パーソナル設定取得サービスの呼出し回数削減）
            ' ### GetPersonalメソッド呼出しを削除（パーソナル設定は本メソッド内で取得）
            '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　初期表示　Start

            'actionMethodName = "GetPersonal"
            'UpdateProcessingFlagToFalse()
            'qc001F04FormDto = DirectCast(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
            '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　初期表示　End

            ' ### DEL-END KATO 2022/08/31 性能改善（パーソナル設定取得サービスの呼出し回数削減）

            '2022.07.04 #ST 11057 ADD-DEL 1ページあたりの件数がタブ移動した後戻ってしまう修正
            'Me.txtIchiPageNoKensuu.Text = qc001F04FormDto.PerPageSize
            '2022.07.04 #ST 11057 ADD-END 1ページあたりの件数がタブ移動した後戻ってしまう修正

            Me.changedFlg = True

            '#11292 ADD start
            Dim form00Dto = SharedComClient.InstanceData.QC001F00FormDTO
            UpdateProcessingFlagToFalse()
            Me.tayoDisptype = DirectCast(ExecuteAction(GetType(QC001F00Action).AssemblyQualifiedName, "GetTayoDisptype", form00Dto, Nothing), String)

            UpdateProcessingFlagToFalse()
            Me.bupDisptype = DirectCast(ExecuteAction(GetType(QC001F00Action).AssemblyQualifiedName, "GetBupDisptype", form00Dto, Nothing), String)
            '#11292 ADD end

            '#9084 GOMADA 2022/09/08 UPDATE START
            '画面の保守区分を更新
            Me.cmbHoshuKbnCombo.SelectedValue = qc001F04FormDto.CmbHoshuKbn
            '#9084 GOMADA 2022/09/08 UPDATE END

            'ToolTipを作成する
            Using ToolTip1 = New ToolTip()
                'フォームがアクティブでない時でもToolTipを表示する
                ToolTip1.ShowAlways = True

                ToolTip1.SetToolTip(SplitContainer1, "一覧の境界線をドラッグすると一覧のサイズを変更できます。")
            End Using

            '#11622 参照モードのコントロール制御を追加 Start
            Select Case SharedComClient.InstanceData.QC001F00FormDTO.HdnSyoriMode
                '        ' 内部＿処理モードが"1"（作成モード）、"4"（デモ貸出モード）の場合
                'Case Consts.SyoriMode.NewAdd, Consts.SyoriMode.DemoKasidasi
                '    Me.InitControlDataRead()
                '        ' 内部＿モードが"2"(訂正モード)、"3"（変更・解約モード）、"5"(デモ売上切替モード)の場合
                'Case Consts.SyoriMode.Teisei, Consts.SyoriMode.Change, Consts.SyoriMode.DemoKirikae
                '    Me.InitControlTeisei()
                '        '内部＿モードが"6"(申請モード)
                'Case Consts.SyoriMode.Sinsei
                '"8"(参照モード)の場合
                Case Consts.SyoriMode.Reference
                    Me.InitControlReference()
                    '#11292 start
                '"1"（作成モード）の場合
                Case Consts.SyoriMode.NewAdd

                    'ステータス先行時,「画面制御パターン：参照のみ」と同じ
                    If String.Equals(Me.tayoDisptype, Consts.TayoDisptype.Disable) Then
                        Me.InitControlStatus()
                    End If
                    '#11292 end
                    '#11308_Start
                    'Case Consts.SyoriMode.InyoAdd
                    'InyoRyokinSaiKeisan()
                    '#11308_End
            End Select
            '#11622 参照モードのコントロール制御を追加 End

            '#11308_Start
            If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.InyoAdd) Then
                InyoRyokinSaiKeisan()
            End If
            '#11308_End

            ' ### DEL-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
            ' ### イベントハンドル再開はForm_Loadの最後に実行する
            'イベントハンドルを再開
            'Me.ResumeHandler()
            ' ### DEL-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            ' ST先行検証 #10434 ADD START
            actionName = GetType(SeigoCheck04Action).AssemblyQualifiedName
            actionMethodName = "RequiredChildMenuCheck"
            UpdateProcessingFlagToFalse()
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
            ' ST先行検証 #10434 ADD END

            ' ### ADD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
            ' ### イベントハンドル再開はForm_Loadの最後に実行する
            ' スプレッド（明細部と合計部）の縦幅の調整を呼出す
            Me.QC001F04_HeightChange(sender, e)
            'イベントハンドルを再開
            Me.ResumeHandler()
            ' ### ADD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            ClientLogUtil.Logger.DebugAP("QC001F04Form:QC001F04Form_Load end")
            '終了処理を実行する
            EndProcess()
        End Sub

        ''' <summary>
        ''' 画面＿明細表示設定_設置先コンボロストフォーカス
        ''' </summary>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub cmbMeisaiHyojiSetteiSettisakiCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMeisaiHyojiSetteiSettisakiCombo.SelectedIndexChanged
        Private Sub cmbMeisaiHyojiSetteiSettisakiCombo_SelectedIndexChanged(sender As Object, e As EventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
            If Not Me.changedFlg Then
                Exit Sub
            End If

            Me.changedFlg = False
            Me.BindFormToDto()
            For rowNo = 0 To Me.sprM1MenuIchiran_Sheet1.RowCount - 1
                Me.sprM1MenuIchiran_Sheet1.Rows(rowNo).Visible = True
            Next

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "CmbMeisaiHyojiSetteiSettisakiCombo_LostFocus"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            ' 障害_ST先行検証 #10558 ADD START
            ' 補正のタイミング - 一覧の設置先を変える時
            ' 参照モード以外の場合
            If Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                ' 値増しチェック
                actionMethodName = "NemashiCheck"
                UpdateProcessingFlagToFalse()
                ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
            End If
            ' 障害_ST先行検証 #10558 ADD END

            ' 画面表示
            Me.Paging()

            ' 2021/08/12 #4813 STA
            'If Not String.Equals(cmbMeisaiHyojiSetteiSettisakiCombo.Text, Consts.zente) Then
            '    For rowNo = 0 To Me.sprM1MenuIchiran_Sheet1.RowCount - 1
            '        If Not Me.sprM1MenuIchiran_Sheet1.Cells(rowNo, buppanEnum.cmbM1SettisakiCombo).Text.Equals(cmbMeisaiHyojiSetteiSettisakiCombo.Text) AndAlso
            '           Not String.IsNullOrEmpty(Me.sprM1MenuIchiran_Sheet1.Cells(rowNo, buppanEnum.txtM1MenuNo).Text) Then
            '            Me.sprM1MenuIchiran_Sheet1.Rows(rowNo).Visible = False
            '        End If
            '    Next

            'End If
            ' 2021/08/12 #4813 END

            'ST1_#3268
            For rowNo = 0 To Me.sprM1MenuIchiran_Sheet1.RowCount - 1
                If Me.sprM1MenuIchiran_Sheet1.Rows(rowNo).Visible Then
                    Me.sprM1MenuIchiran_Sheet1.SetActiveCell(rowNo, 0)
                    Exit For
                End If
            Next

            Me.changedFlg = True
        End Sub

        ''' <summary>
        ''' 画面＿合計表示設定_保守ラジオ変更
        ''' </summary>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub rdoGokeiHyojiSetteiHoshuRadio_Checked(sender As Object, e As EventArgs) Handles rdoGokeiHyojiSetteiHoshuRadio.CheckedChanged
        Private Sub rdoGokeiHyojiSetteiHoshuRadio_Checked(sender As Object, e As EventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            If Not Me.changedFlg Then
                Exit Sub
            End If

            Me.BindFormToDto()
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "RdoGokeiHyojiSetteiHoshuRadio_Checked"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            ' 画面表示
            Me.Paging()

        End Sub

        ''' <summary>
        ''' 画面＿保守区分コンボロストフォーカス
        ''' </summary>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub cmbHoshuKbnCombo_LostFocus(sender As Object, e As EventArgs) Handles cmbHoshuKbnCombo.SelectedIndexChanged
        Private Sub cmbHoshuKbnCombo_LostFocus(sender As Object, e As EventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            If Not Me.changedFlg Then
                Exit Sub
            End If

            If Not String.Equals(Me.cmbHoshuKbnCombo.SelectedValue, qc001F04FormDto.CmbHoshuKbn) Then
                Dim cmbHoshuKbnCombo As String
                cmbHoshuKbnCombo = qc001F04FormDto.CmbHoshuKbn
                Me.BindFormToDto()
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "CmbHoshuKbnCombo_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                If Not qc001F04FormDto.ContinueFlg Then
                    qc001F04FormDto.CmbHoshuKbn = cmbHoshuKbnCombo
                    qc001F04FormDto.ContinueFlg = True
                    Me.cmbHoshuKbnCombo.SelectedValue = qc001F04FormDto.CmbHoshuKbn
                End If

                '画面表示
                Me.Paging()
            End If

        End Sub

        ''' <summary>
        ''' 画面＿保守料金算出基準日ロストフォーカス
        ''' </summary>
        Private Sub sprHoshuRyokinSansyutsuKijunDate_LostFocus(sender As Object, e As EventArgs) Handles sprHoshuRyokinSansyutsuKijunDate.Leave

            If Not Me.changedFlg Then
                Exit Sub
            End If

            changedFlg = False
            Dim time = String.Format("{0:yyyy/MM/dd}", Me.sprHoshuRyokinSansyutsuKijunDate.Value)
            If Not String.Equals(time, qc001F04FormDto.SprHoshuRyokinSansyutsuKijunDate) Then
                Dim times As Date
                If Not DateTime.TryParseExact(String.Format("{0:yyyy/MM/dd}", Me.sprHoshuRyokinSansyutsuKijunDate.Value).Replace(Consts.slash, Consts.blank), "yyyyMMdd", New CultureInfo("ja-JP", True), DateTimeStyles.None, times) Then
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0694))
                    Me.sprHoshuRyokinSansyutsuKijunDate.Value = CommUtility.ParseDate(qc001F04FormDto.SprHoshuRyokinSansyutsuKijunDate, "yyyyMMdd")
                    ' #3480 START
                    Me.changedFlg = True
                    ' #3480 END
                    Return
                End If

                qc001F04FormDto.SprHoshuRyokinSansyutsuKijunDate = time
                qc001F04FormDto.NaibuSprHoshuRyokinSansyutsuKijunDate = qc001F04FormDto.SprHoshuRyokinSansyutsuKijunDate.Replace(Consts.slash, Consts.blank)
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "SprHoshuRyokinSansyutsuKijunDate_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                ' 設置先コンボ編集
                Me.SetteiSettisakiCombo()

                ' 画面表示
                Me.Paging()

                ' QC001F00Formのアドオンチェックボタン制御
                Me.CallQC001F00FormAddonCheckSeigyo()
            End If

            changedFlg = True

        End Sub

        ''' <summary>
        ''' 画面＿多拠点ボタンクリック
        ''' </summary>
        Private Sub btnTaKyoten_Click(sender As Object, e As EventArgs) Handles btnTaKyoten.Click

            '開始処理を行う
            InitProcess()
            If Not Me.changedFlg Then
                Exit Sub
            End If
            Me.changedFlg = False
            Dim dto As New QC001F04FormDto
            dto = qc001F04FormDto

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnTaKyoten_Click"
            ' 選択行の取得
            Me.GetRowIndex()
            '2022.08.26 DEL-START 多拠点ではオプションメニュー選択時も動かすことは可能
            'For Each num As Integer In qc001F04FormDto.SelectedRowIndex
            '    If Not Me.CheckMenuChoose(qc001F04FormDto, num) Then
            '        Me.changedFlg = True
            '        MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1485))
            '        Return
            '    End If
            'Next
            '2022.08.26 DEL-END   多拠点ではオプションメニュー選択時も動かすことは可能
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            '2022-11-13 ADD START #13370
            qc001F04FormDto.EntireRowSelected = False
            '2022-11-13 ADD END #13370

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()

            '画面表示
            Me.Paging()

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()
            Me.changedFlg = True

            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 選択行のNoを取得
        ''' </summary>
        ''' <param name="rowCount">選択行（先頭は0から）</param>
        Private Function getRowNo(rowCount As Integer) As Integer
            '#6472 start
            '    Return CInt(Me.sprM1MenuIchiran_Sheet1.Cells(rowCount, 0).Text) - 1
            Return CInt(Me.sprM1MenuIchiran.ActiveSheet.Cells(rowCount, 0).Value) - 1
            '#6472 end
        End Function
        '#6472対応 layout修正 START
        ''' <summary>
        ''' 画面の縦幅が変更する時、スプレッド（明細部と合計部）の縦幅の調整
        ''' </summary>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub QC001F04_HeightChange(sender As Object, e As EventArgs) Handles Me.Resize
        Private Sub QC001F04_HeightChange(sender As Object, e As EventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            '開始処理を行う。
            InitProcess()

            sprM1MenuIchiran.Height = (Me.Height - 200) / 2
            '12814　6472 横展開 begin
            'sprM2GokeiIchiran.Height = (Me.Height - 200) / 2
            '12814　6472 横展開 end
            '終了処理を行う。
            EndProcess()
        End Sub
        '#6472対応 layout修正 END
        ''' <summary>
        ''' Ｍ１＿ロストフォーカス
        ''' </summary>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub M1_LostFocus(sender As Object, e As SheetViewEventArgs) Handles sprM1MenuIchiran_Sheet1.CellChanged
        Private Sub M1_LostFocus(sender As Object, e As SheetViewEventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            If Not Me.changedFlg Then
                Exit Sub
            End If

            'セル以外変更の場合
            If Not cellChangeFlg Then
                cellChangeFlg = True
                Exit Sub
            Else
                cellChangeFlg = False
            End If

            qc001F04FormDto.ContinueFlg = True
            Me.changedFlg = False
            ' 選択行の取得
            qc001F04FormDto.SelectedRowIndex = New List(Of Integer)
            '8259 Start
            qc001F04FormDto.SelectedRowIndex.Add(getRowNo(e.Row))
            '8259 End
            ' Ｍ１＿メニュー番号ロストフォーカス
            If e.Column.Equals(1) Then

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM1MenuNo_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                ' Ｍ１＿年額定価ロストフォーカス
            ElseIf e.Column.Equals(11) Then

                Me.NaibuCalculateMesai(e.Column, (CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row)
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM1NengakuTeika_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                ' 背景色を変更する
                If Not qc001F04FormDto.ContinueFlg Then
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.Red
                Else
                    If Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Locked Then
                        Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.White
                    End If
                End If

                ' Ｍ１＿年額売価単価ロストフォーカス
            ElseIf e.Column.Equals(13) Then

                Me.NaibuCalculateMesai(e.Column, (CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row)
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM1NengakuBinTnk_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                ' 背景色を変更する
                If Not qc001F04FormDto.ContinueFlg Then
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.Red
                Else
                    If Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Locked Then
                        Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.White
                    End If
                End If

                ' Ｍ１＿月額売価単価ロストフォーカス
            ElseIf e.Column.Equals(17) Then

                Me.NaibuCalculateMesai(e.Column, (CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row)
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM1GetsugakuBinTnk_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                ' 背景色を変更する
                If Not qc001F04FormDto.ContinueFlg Then
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.Red
                Else
                    If Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Locked Then
                        Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.White
                    End If
                End If
                'Ｍ１＿原価区分
            ElseIf e.Column.Equals(23) Then
                If String.Equals("課", Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column - 20).Text) AndAlso
                    String.Equals("申請", Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Text) Then
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column + 1).Locked = False
                End If
                For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).M1GnkKbn
                    If String.Equals(ComboxOptionDto.Name, Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Text) Then
                        qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).M1GnkKbnChoose = ComboxOptionDto.Code
                        Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                        Dim actionMethodName As String = "CmbM1GnkKbn_LostFocus"
                        UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                        qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                        ' ST#10280 START
                        Dim check123 = ApplicationScope.InstanceData.GetValue("Check123.ChangeKBN")
                        If String.Equals(check123, "CANCEL") Then
                            Me.sprM1MenuIchiran_Sheet1.SetActiveCell(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow, 23)
                            ApplicationScope.InstanceData.PutValue("Check123.ChangeKBN", "")
                        End If
                        ' ST#10280 END
                        Exit For
                    End If
                Next
                ' Ｍ１＿標準原価ロストフォーカス
            ElseIf e.Column.Equals(24) Then

                Me.NaibuCalculateMesai(e.Column, (CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row)
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM1HyojunGnk_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                'ST1_#4643 START
                ' 背景色を変更する
                If Not qc001F04FormDto.ContinueFlg Then
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.Red
                Else
                    If Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Locked Then
                        Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).BackColor = Drawing.Color.White
                    End If
                End If
                If Not Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, 13).Locked Then
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, 13).BackColor = Drawing.Color.Red
                End If
                'ST1_#4643 END

                '#2022/09/14 #13550 ADD START Ｍ１＿月額無償ロストフォーカス処理追加
                ' Ｍ１＿月額無償ロストフォーカス
            ElseIf e.Column.Equals(27) Then
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "CmbM1GetsugakuMusyoCombo_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                '#2022/09/14 #13550 ADD END

                ' Ｍ１＿設置先コンボロストフォーカス
            ElseIf e.Column.Equals(28) Then

                '20220808 ST#12228 ADD-START
                'メニュー番号のない新規行は処理しない
                Dim selectedIndex As Integer = qc001F04FormDto.SelectedRowIndex.First
                If (qc001F04FormDto.SprM1MenuIchiran.Count <= selectedIndex) OrElse
                   (String.IsNullOrEmpty(qc001F04FormDto.SprM1MenuIchiran(selectedIndex).M1MenuNo)) Then
                    Me.changedFlg = True
                    Return
                End If
                '20220808 ST#12228 ADD-END

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "CmbM1SettisakiCombo_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                ' Ｍ１＿グループコンボロストフォーカス
            ElseIf e.Column.Equals(29) Then
                If Not IsNothing(qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).CmbM1GroupCombo) Then
                    For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).CmbM1GroupCombo
                        If String.Equals(ComboxOptionDto.Name, qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).CmbM1GroupComboChoose) Then
                            qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).CmbM1GroupComboCode = ComboxOptionDto.Code
                        End If
                    Next
                End If
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "CmbM1GroupCombo_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            End If

            If Not qc001F04FormDto.ContinueFlg Then
                qc001F04FormDto.SprM1MenuIchiran = CType(Copy(qc001F04FormDto.SprM1MenuIchiranBk), BindingList(Of QC001F04M1Dto))
                qc001F04FormDto.ContinueFlg = True
                Me.Paging(False)
                Me.changedFlg = True
                Return
            End If

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()
            Me.Paging()
            Me.changedFlg = True

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()

        End Sub

        ''' <summary>
        ''' Ｍ１＿クリック
        ''' </summary>
        Private Sub M1_Checked(ByVal sender As Object, ByVal e As FarPoint.Win.Spread.EditorNotifyEventArgs) Handles sprM1MenuIchiran.ButtonClicked

            '開始処理を行う
            InitProcess()

            If Not Me.changedFlg Then
                Exit Sub
            End If
            Me.changedFlg = False
            ' 選択行の取得
            qc001F04FormDto.SelectedRowIndex = New List(Of Integer)
            qc001F04FormDto.SelectedRowIndex.Add(getRowNo(e.Row))
            ' Ｍ１＿アドオン（必須）クリック
            If e.Column.Equals(7) Then

                '#11292 ADD start
                '見積・契約入力の物販表示状態取得処理 = [1：非活性]の場合、
                ' 推奨構成で選択した内容を[物販明細タブ]には反映しないため、確認メッセージ（IKB030）を表示する。
                If String.Equals(Me.bupDisptype, Consts.BupDisptype.Disable) Then
                    Dim ret = MessageDialogUtil.ShowInfo(MessageUtil.GetDialogProperty(BusinessMessageConst.IKB030))
                    'メッセージで[キャンセル]を選択した場合、画面を表示しない。
                    If String.Equals(ret, Consts.MessageSelect.Cancel) Then
                        EndProcess()
                        Exit Sub
                    End If
                End If
                '#11292 ADD end

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM1AddonHissu_Click"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                ' Ｍ１＿アドオン（推奨）クリック
            ElseIf e.Column.Equals(8) Then

                '#11292 ADD start
                '見積・契約入力の物販表示状態取得処理 = [1：非活性]の場合、
                ' 推奨構成で選択した内容を[物販明細タブ]には反映しないため、確認メッセージ（IKB030）を表示する。
                If String.Equals(Me.bupDisptype, Consts.BupDisptype.Disable) Then
                    Dim ret = MessageDialogUtil.ShowInfo(MessageUtil.GetDialogProperty(BusinessMessageConst.IKB030))
                    'メッセージで[キャンセル]を選択した場合、画面を表示しない。
                    If String.Equals(ret, Consts.MessageSelect.Cancel) Then
                        EndProcess()
                        Exit Sub
                    End If
                End If
                '#11292 ADD end

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM1AddonSuisho_Click"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                ' Ｍ１＿委託希望チェック
            ElseIf e.Column.Equals(9) Then
                ' Ｍ１＿委託希望
                If String.Equals(qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).M1ItakuKibo, Consts.checKbox.checktrue) Then
                    ' Ｍ１＿委託希望チェックＯＦＦ
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value = False
                    qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).M1ItakuKibo = Consts.checKbox.checkfalse

                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "LblM1ItakuKibo_Unchecked"
                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)

                Else
                    ' Ｍ１＿委託希望チェックＯＮ
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value = True
                    qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).M1ItakuKibo = Consts.checKbox.checktrue

                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "LblM1ItakuKibo_Checked"
                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)

                End If

                ' Ｍ１＿無償（初期費用）チェック
            ElseIf e.Column.Equals(19) Then
                ' Ｍ１＿無償（初期費用）
                qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).M1MusyoShokiHiyo = CType(Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value, String)
                If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value.ToString, Consts.checKbox.checkfalse) Then
                    ' Ｍ１＿無償（初期費用）チェックＯＦＦ
                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "LblM1MusyoShokiHiyo_Unchecked"
                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                Else
                    ' Ｍ１＿無償（初期費用）チェックＯＮ
                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "LblM1MusyoShokiHiyo_Checked"
                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                End If

                ' Ｍ１＿無償（随時費用）チェック
            ElseIf e.Column.Equals(21) Then
                ' Ｍ１＿無償（随時費用）
                qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).M1MusyoZuijiHiyo = CType(Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value, String)
                '#6731
                If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value.ToString, Consts.checKbox.checkfalse) Then
                    ' Ｍ１＿無償（随時費用）チェックＯＦＦ
                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "LblM1MusyoZuijiHiyo_Unchecked"
                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                Else
                    ' Ｍ１＿無償（随時費用）チェックＯＮ
                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "LblM1MusyoZuijiHiyo_Checked"
                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                End If
                ' Ｍ１＿サブタイトルチェックチェック
            ElseIf e.Column.Equals(30) Then
                ' Ｍ１＿サブタイトル
                qc001F04FormDto.SprM1MenuIchiran((CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + e.Row).M1SubTtl = CType(Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value, Boolean)
                If Not CType(Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value, Boolean) Then
                    'ST1_#7055_7022 START
                    '' Ｍ１＿サブタイトルチェックチェックＯＦＦ
                    'Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    'Dim actionMethodName As String = "ChkM1SubTtlCheck_Unchecked"
                    'UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    'qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                    Dim dialogResult = MessageDialogUtil.ShowInfo(GetDialogProperty(BusinessMessageConst.IK0195))
                    If String.Equals(dialogResult, Consts.MessageSelect.Hai) Then
                        ' Ｍ１＿サブタイトルチェックチェックＯＦＦ
                        Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                        Dim actionMethodName As String = "ChkM1SubTtlCheck_Unchecked"
                        UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                        qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                    ElseIf String.Equals(dialogResult, Consts.MessageSelect.Iie) Then
                        ' Ｍ１＿サブタイトルチェックチェックＯＮ
                        Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                        Dim actionMethodName As String = "ChkM1SubTtlCheck_Checked"
                        UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                        qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                    End If
                    'ST1_#7055_7022 END
                Else
                    ' Ｍ１＿サブタイトルチェックチェックＯＮ
                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "ChkM1SubTtlCheck_Checked"
                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                End If
                'ST#7055 決定ボタンが押下されたかに関わらず、値の入力によってチェックを行う
                If (String.IsNullOrEmpty(qc001F04FormDto.SprM1MenuIchiran(e.Row).M1SubTtl2)) Then
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value = False
                Else
                    Me.sprM1MenuIchiran_Sheet1.Cells(e.Row, e.Column).Value = True
                End If

            End If

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()

            '画面表示
            Me.Paging()

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()
            Me.changedFlg = True

            '#11616② Start
            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()
            '#11616② End

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿グループ変更ボタンクリック
        ''' </summary>
        Private Sub btnGroupHenko_Click(sender As Object, e As EventArgs) Handles btnGroupHenko.Click

            '開始処理を行う
            InitProcess()

            If Not Me.changedFlg Then
                Exit Sub
            End If

            ' 明細チェック:01 「メニュー明細エリア」で１行も選択されていない場合エラー
            '#6144
            If sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow < 0 Then
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                Exit Sub
            End If

            ' 選択行の取得
            Me.GetRowIndex()

            Dim senFlg As Boolean = False
            If qc001F04FormDto.SelectedRowIndex.Count > 0 Then
                Dim startnum As Integer = qc001F04FormDto.SelectedRowIndex(0)
                For Each num As Integer In qc001F04FormDto.SelectedRowIndex
                    '4489
                    If Not String.IsNullOrEmpty(qc001F04FormDto.SprM1MenuIchiran(num).M1MenuNo) Then
                        senFlg = True
                    End If
                    '#4491 EK1436削除
                    'If Not String.Equals(num, startnum) Then
                    '    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1436))
                    '    Exit Sub
                    'End If
                    startnum += 1
                Next
            Else
                Exit Sub
            End If

            '4489
            If Not senFlg Then
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                Exit Sub
            End If

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnGroupHenko_Click"
            Dim buttonId As New List(Of String)
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
            If Not qc001F04FormDto.ContinueFlg Then
                qc001F04FormDto.SprM1MenuIchiran = CType(Copy(qc001F04FormDto.SprM1MenuIchiranBk), BindingList(Of QC001F04M1Dto))
                qc001F04FormDto.ContinueFlg = True
                Return
            End If

            '画面表示
            Me.Paging()

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿値引設定_メニュー別ラジオ変更
        ''' </summary>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub rdoNebikiSetteiMenuBetsuRadio_Change(sender As Object, e As EventArgs) Handles rdoNebikiSetteiMenuBetsuRadio.CheckedChanged
        Private Sub rdoNebikiSetteiMenuBetsuRadio_Change(sender As Object, e As EventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            If Not Me.changedFlg Then
                Exit Sub
            End If
            Me.BindFormToDto()
            '画面表示
            Me.Paging()

        End Sub

        ''' <summary>
        ''' Ｍ２＿ロストフォーカス
        ''' </summary>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub lblM2GokeiranNengakuHiyo_LostFocus(sender As Object, e As SheetViewEventArgs) Handles sprM2GokeiIchiran_Sheet1.CellChanged
        Private Sub lblM2GokeiranNengakuHiyo_LostFocus(sender As Object, e As SheetViewEventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            If Not Me.changedFlg Then
                Exit Sub
            End If

            'セル以外変更の場合
            If Not cellChangeFlg Then
                cellChangeFlg = True
                Exit Sub
            Else
                cellChangeFlg = False
            End If

            Me.changedFlg = False
            ' 選択行の取得
            qc001F04FormDto.SelectedRowIndex = New List(Of Integer)
            qc001F04FormDto.SelectedRowIndex.Add(getRowNo(e.Row))

            ' Ｍ２＿合計欄_年額費用ロストフォーカス
            If e.Column.Equals(4) Then

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM2GokeiranNengakuHiyo_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            End If

            ' Ｍ２＿月額換算後欄_月額費用ロストフォーカス
            If e.Column.Equals(13) Then

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "LblM2GetsugakuKansangoranGetsugakuHiyo_LostFocus"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            End If
            Me.Paging()

            Me.changedFlg = True

        End Sub

        ''' <summary>
        ''' 画面＿メニュー選択ボタンクリック
        ''' </summary>
        Private Sub btnMenuSentaku_Click(sender As Object, e As EventArgs) Handles btnMenuSentaku.Click

            '開始処理を行う
            InitProcess()

            If Not Me.changedFlg Then
                Exit Sub
            End If
            Me.changedFlg = False

            ' 障害_ST先行検証 #10558 ADD START
            ' 補正のタイミング - たよ明細に戻る時
            Dim menuInSheetExist As Boolean = False
            Dim settisaki As String = Me.cmbMeisaiHyojiSetteiSettisakiCombo.Text
            If (settisaki.Equals(Consts.zente)) AndAlso
                qc001F04FormDto.SprM1MenuIchiran.Any(Function(p) Not String.IsNullOrEmpty(p.CmbM1SettisakiComboChoose)) Then
                menuInSheetExist = True
            End If
            If (Not settisaki.Equals(Consts.zente)) AndAlso
                qc001F04FormDto.SprM1MenuIchiran.Any(Function(p) settisaki.Equals(p.CmbM1SettisakiComboChoose)) Then
                menuInSheetExist = True
            End If
            ' 障害_ST先行検証 #10558 ADD END

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnMenuSentaku_Click"

            ' 選択行の取得
            Me.GetRowIndex()

            For Each num As Integer In qc001F04FormDto.SelectedRowIndex
                If Not Me.CheckMenuChoose(qc001F04FormDto, num) Then
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1485))
                    ' #3480 START
                    Me.changedFlg = True
                    ' #3480 END
                    Return
                End If
            Next
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            If Not qc001F04FormDto.ContinueFlg Then
                qc001F04FormDto.ContinueFlg = True
            End If

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()

            ' 障害_ST先行検証 #10558 ADD START
            ' 補正のタイミング - たよ明細に戻る時
            ' 参照モード以外の場合
            If Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                If Not menuInSheetExist Then
                    ' 値増しチェック
                    actionMethodName = "NemashiCheck"
                    UpdateProcessingFlagToFalse()
                    ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
                End If

                ' 2022/10/26 ADD-START #9282 原価区分取得を呼び出す
                actionMethodName = "GenkaKbnCheck"
                UpdateProcessingFlagToFalse()
                ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
                ' 2022/10/26 ADD-END #9282
            End If
            ' 障害_ST先行検証 #10558 ADD END

            '#12150 2022.08.08 START
            Me.bFlag = True
            '#12150 2022.08.08 END

            '画面表示
            Me.Paging()

            ' 2021/08/12 #4813 STA
            'If Not String.Equals(cmbMeisaiHyojiSetteiSettisakiCombo.Text, Consts.zente) Then
            '    For rowNo = 0 To Me.sprM1MenuIchiran_Sheet1.RowCount - 1
            '        Me.sprM1MenuIchiran_Sheet1.Rows(rowNo).Visible = True
            '        If Not Me.sprM1MenuIchiran_Sheet1.Cells(rowNo, buppanEnum.cmbM1SettisakiCombo).Text.Equals(cmbMeisaiHyojiSetteiSettisakiCombo.Text) AndAlso
            '           Not String.IsNullOrEmpty(Me.sprM1MenuIchiran_Sheet1.Cells(rowNo, buppanEnum.txtM1MenuNo).Text) Then
            '            Me.sprM1MenuIchiran_Sheet1.Rows(rowNo).Visible = False
            '        End If
            '    Next
            'End If
            ' 2021/08/12 #4813 END

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()
            Me.changedFlg = True

            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 右クリック：メニュー選択
        ''' </summary>
        Private Sub rightClickMenuSentaku_Click(sender As Object, e As EventArgs) Handles RightClickMenuSentaku.Click

            '開始処理を行う
            InitProcess()

            Dim menu As Windows.Forms.ToolStripMenuItem = CType(sender, Windows.Forms.ToolStripMenuItem)
            Try
                menu.Enabled = False

                If Not Me.changedFlg Then
                    Exit Sub
                End If
                Me.changedFlg = False

                ' 障害_ST先行検証 #10558 ADD START
                ' 補正のタイミング - たよ明細に戻る時
                Dim menuInSheetExist As Boolean = False
                Dim settisaki As String = Me.cmbMeisaiHyojiSetteiSettisakiCombo.Text
                If (settisaki.Equals(Consts.zente)) AndAlso
                qc001F04FormDto.SprM1MenuIchiran.Any(Function(p) Not String.IsNullOrEmpty(p.CmbM1SettisakiComboChoose)) Then
                    menuInSheetExist = True
                End If
                If (Not settisaki.Equals(Consts.zente)) AndAlso
                qc001F04FormDto.SprM1MenuIchiran.Any(Function(p) settisaki.Equals(p.CmbM1SettisakiComboChoose)) Then
                    menuInSheetExist = True
                End If
                ' 障害_ST先行検証 #10558 ADD END

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "RightClickMenuSentaku_Click"

                ' 選択明細行の情報を設定する。
                Dim buttonId As New List(Of String)
                ' 選択行の取得
                Me.GetRowIndex()

                For Each num As Integer In qc001F04FormDto.SelectedRowIndex
                    If Not Me.CheckMenuChoose(qc001F04FormDto, num) Then
                        MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1485))
                        Return
                    End If
                Next
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                If Not qc001F04FormDto.ContinueFlg Then
                    qc001F04FormDto.ContinueFlg = True
                End If

                ' 設置先コンボ編集
                Me.SetteiSettisakiCombo()

                ' 障害_ST先行検証 #10558 ADD START
                ' 補正のタイミング - たよ明細に戻る時
                ' 参照モード以外の場合
                If Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                    If Not menuInSheetExist Then
                        ' 値増しチェック
                        actionMethodName = "NemashiCheck"
                        UpdateProcessingFlagToFalse()
                        ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
                    End If
                End If
                ' 障害_ST先行検証 #10558 ADD END

                '画面表示
                Me.Paging()

                ' QC001F00Formのアドオンチェックボタン制御
                Me.CallQC001F00FormAddonCheckSeigyo()
                Me.changedFlg = True
            Finally
                menu.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿推奨構成ボタン
        ''' </summary>
        Private Sub btnSuishoKosei_Click(sender As Object, e As EventArgs) Handles btnSuishoKosei.Click

            '開始処理を行う
            InitProcess()

            '#11292 ADD start
            '見積・契約入力の物販表示状態取得処理 = [1：非活性]の場合、
            ' 推奨構成で選択した内容を[物販明細タブ]には反映しないため、確認メッセージ（IKB030）を表示する。
            If String.Equals(Me.bupDisptype, Consts.BupDisptype.Disable) Then
                Dim ret = MessageDialogUtil.ShowInfo(MessageUtil.GetDialogProperty(BusinessMessageConst.IKB030))
                'メッセージで[キャンセル]を選択した場合、画面を表示しない。
                If String.Equals(ret, Consts.MessageSelect.Cancel) Then
                    EndProcess()
                    Exit Sub
                End If
            End If
            '#11292 ADD end

            If Not Me.changedFlg Then
                Exit Sub
            End If

            '#9084 GOMADA 2022/09/08 UPDATE START1
            '推奨構成保守区分チェックフラグを立てる
            qc001F04FormDto.msuishoHoshkbnCheckFlg = True
            '#9084 GOMADA 2022/09/08 UPDATE END

            ' 選択行の取得
            Me.GetRowIndex()

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnSuishoKosei_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            ' 画面表示
            Me.Paging()
            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()

            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()

            '#9084 GOMADA 2022/09/08 UPDATE START
            '画面の保守区分を更新
            Me.cmbHoshuKbnCombo.SelectedValue = qc001F04FormDto.CmbHoshuKbn
            '#9084 GOMADA 2022/09/08 UPDATE END
            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 右クリック：推奨構成検索
        ''' </summary>
        Private Sub rightClickSuishoKosei_Click(sender As Object, e As EventArgs) Handles RightClickSuishoKosei.Click

            '開始処理を行う
            InitProcess()

            '#11292 ADD start
            '見積・契約入力の物販表示状態取得処理 = [1：非活性]の場合、
            ' 推奨構成で選択した内容を[物販明細タブ]には反映しないため、確認メッセージ（IKB030）を表示する。
            If String.Equals(Me.bupDisptype, Consts.BupDisptype.Disable) Then
                Dim ret = MessageDialogUtil.ShowInfo(MessageUtil.GetDialogProperty(BusinessMessageConst.IKB030))
                'メッセージで[キャンセル]を選択した場合、画面を表示しない。
                If String.Equals(ret, Consts.MessageSelect.Cancel) Then
                    EndProcess()
                    Exit Sub
                End If
            End If
            '#11292 ADD end

            Dim menu As Windows.Forms.ToolStripMenuItem = CType(sender, Windows.Forms.ToolStripMenuItem)
            Try
                menu.Enabled = False
                If Not Me.changedFlg Then
                    Exit Sub
                End If

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "RightClickSuishoKosei_Click"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                '画面表示
                Me.Paging()
                ' QC001F00Formのアドオンチェックボタン制御
                Me.CallQC001F00FormAddonCheckSeigyo()
            Finally
                menu.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 右クリック：最新単価取込
        ''' </summary>
        Private Sub rightClickSaishinTankaTorikomi_Click(sender As Object, e As EventArgs) Handles RightClickSaishinTankaTorikomi.Click

            '開始処理を行う
            InitProcess()

            Dim menu As Windows.Forms.ToolStripMenuItem = CType(sender, Windows.Forms.ToolStripMenuItem)
            Try
                menu.Enabled = False


                ' 選択行がない場合
                ''#4491
                If sprM1MenuIchiran_Sheet1.GetSelections.Count = 0 Then
                    Exit Sub
                End If

                ' 選択行の取得
                Me.GetRowIndex()

                If qc001F04FormDto.SelectedRowIndex.Count = 0 Then
                    Exit Sub
                End If

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "RightClickSaishinTankaTorikomi_Click"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                ' 画面表示
                Me.Paging()
            Finally
                menu.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿料金再計算ボタンクリック
        ''' </summary>
        Public Sub btnRyokinSaiKeisan_Click(sender As Object, e As EventArgs) Handles btnRyokinSaiKeisan.Click

            '開始処理を行う
            InitProcess()

            '#6144
            If sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow < 0 Then
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                Exit Sub
            End If
            Me.changedFlg = False
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnRyokinSaiKeisan_Click"

            Dim buttonId As New List(Of String)
            ' 選択行の取得
            Me.GetRowIndex()

            If qc001F04FormDto.SelectedRowIndex.Count = 0 OrElse (qc001F04FormDto.SelectedRowIndex.Count = 1 AndAlso String.IsNullOrEmpty(qc001F04FormDto.SprM1MenuIchiran(qc001F04FormDto.SelectedRowIndex(0)).M1NaibuNo)) Then
                Me.changedFlg = True
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                Exit Sub
            End If

            For Each num As Integer In qc001F04FormDto.SelectedRowIndex
                If Not Me.CheckMenuChoose(qc001F04FormDto, num) Then
                    Me.changedFlg = True
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1485))
                    Return
                End If
            Next
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()

            Me.Paging()

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()
            Me.changedFlg = True

            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 見積引用_たよ明細を開く際、料金再計算を実施する
        ''' </summary>
        Private Sub InyoRyokinSaiKeisan()
            If (qc001F04FormDto.SprM1MenuIchiran.Count - 1) <= 0 Then
                Return
            End If
            '開始処理を行う
            InitProcess()

            Me.changedFlg = False
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnRyokinSaiKeisan_Click"

            Dim numList As New List(Of Integer)

            'qc001F04FormDto.numList.Clear()
            For i = 0 To qc001F04FormDto.SprM1MenuIchiran.Count - 2
                If Not Me.CheckMenuChoose(qc001F04FormDto, i) Then
                    Me.changedFlg = True
                    Continue For
                End If
                numList.Add(i)
            Next
            qc001F04FormDto.numList = numList
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
            If Not IsNothing(qc001F04FormDto.numList) Then
                qc001F04FormDto.numList.Clear()
            End If

            'Dim buttonId As New List(Of String)
            ' 選択行の取得

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()

            Me.Paging()

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()
            'Me.changedFlg = True

            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿数・機器変更ボタンクリック
        ''' </summary>
        Private Sub btnMenuFutai_Click(sender As Object, e As EventArgs) Handles btnMenuFutai.Click

            '開始処理を行う
            InitProcess()

            Me.changedFlg = False
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnMenuFutai_Click"

            ' 選択行の取得
            Me.GetRowIndex()

            For Each num As Integer In qc001F04FormDto.SelectedRowIndex
                If Not Me.CheckMenuChoose(qc001F04FormDto, num) Then
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1485))
                    ' #3480 START
                    Me.changedFlg = True
                    ' #3480 END
                    Return
                End If
            Next
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()

            Me.Paging()

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()
            Me.changedFlg = True

            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿契約付帯入力ボタンクリック
        ''' </summary>
        Private Sub btnFutaiNyuryoku_Click(sender As Object, e As EventArgs) Handles btnFutaiNyuryoku.Click

            '開始処理を行う
            InitProcess()

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnFutaiNyuryoku_Click"
            Dim buttonId As New List(Of String)
            ' 選択行の取得
            Me.GetRowIndex()

            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            Me.Paging()

            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿正誤チェックボタンクリック
        ''' </summary>
        Private Sub btnSeigoCheck_Click(sender As Object, e As EventArgs) Handles btnSeigoCheck.Click

            '開始処理を行う
            InitProcess()

            ' #4669 START
            Dim checkSeigoDto = DirectCast(ApplicationScope.InstanceData.GetValue("CheckAllSeigoDto"), QC001F00CheckSeigoDTO)
            If checkSeigoDto IsNot Nothing Then
                checkSeigoDto.MessageInfoDtoList.Clear()
                checkSeigoDto.MsgIdList.Clear()
            End If
            ' #4669 END
            If (qc001F04FormDto.SprM1MenuIchiran.Count = 0 OrElse
           Not String.IsNullOrEmpty(qc001F04FormDto.SprM1MenuIchiran(qc001F04FormDto.SprM1MenuIchiran.Count - 1).M1MenuNo)) AndAlso
           Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Modify) AndAlso
           Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.DemoKirikae) AndAlso
           Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.DemoKasidasi) AndAlso
           Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Sinsei) AndAlso
           Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Print) AndAlso
           Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.TanNendoUpdate) AndAlso
           Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                If String.Equals(qc001F04FormDto.SprM1MenuIchiran.Count, 1) Then
                    Return
                End If
            Else
                If String.Equals(qc001F04FormDto.SprM1MenuIchiran.Count, 0) Then
                    Return
                End If
            End If

            'ST1_#4801 START
            qc001F04FormDto.ShoriKbn = ""
            'ST1_#4801 END
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnSeigoCheck_Click"
            Dim buttonId As New List(Of String)

            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            '画面表示(値増し対応)
            Me.Paging()

            'QC001F00Formを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)

            'タイトルバーをリフレッシュする
            QC001F00Form.TitleBarRefresh()

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿明細参照ボタンクリック
        ''' </summary>
        Private Sub btnMeisaiSansyo_Click(sender As Object, e As EventArgs) Handles btnMeisaiSansyo.Click

            '開始処理を行う
            InitProcess()

            Dim btn As Windows.Forms.Button = CType(sender, Windows.Forms.Button)
            Try
                btn.Enabled = False

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "BtnMeisaiSansyo_Click"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
            Finally
                btn.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿明細総合計ボタンクリック
        ''' </summary>
        Private Sub btnMeisaiSoGokei_Click(sender As Object, e As EventArgs) Handles btnMeisaiSoGokei.Click

            '開始処理を行う
            InitProcess()

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnMeisaiSoGokei_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿全表示・設定幅ボタン
        ''' </summary>
        Public Sub BtnAllHyojiSetteiHaba_Click(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing) Handles btnAllHyojiSetteiHaba.Click

            '#10551 Insert Start
            If Not String.Equals("btnAllHyojiSetteiHaba", sender.name) AndAlso Not String.Equals(btnAllHyojiSetteiHaba.Text, sender.Text) Then
                Return
            End If
            '#10551 Insert End

            '開始処理を行う
            InitProcess()

            ' #13489 start
            If String.Equals(btnAllHyojiSetteiHaba.Text, Consts.zenbyouji) Then
                ' ボタンは「設定幅」にする。
                btnAllHyojiSetteiHaba.Text = Consts.setteihaba
                '＃5320　START　　　
                ' 列幅の最小値
                'For Each m1Column As Column In sprM1MenuIchiran_Sheet1.Columns

                For i = 0 To sprM1MenuIchiran_Sheet1.Columns.Count - 1
                    'm1Column.Width = m1Column.GetPreferredWidth

                    Dim getPreferredWidth As Integer = sprM1MenuIchiran_Sheet1.Columns(i).GetPreferredWidth
                    Dim columnHeaderLength As Integer = 17
                    If Not sprM1MenuIchiran_Sheet1.ColumnHeader.Cells(0, i).Value Is Nothing Then
                        columnHeaderLength = sprM1MenuIchiran_Sheet1.ColumnHeader.Cells(0, i).Value.ToString.Length * 17
                    End If
                    sprM1MenuIchiran_Sheet1.Columns(i).Width = Math.Max(getPreferredWidth, columnHeaderLength)
                    '＃5320　END
                Next
            ElseIf String.Equals(btnAllHyojiSetteiHaba.Text, Consts.setteihaba) Then
                ' ボタンは「全表示」にする。
                btnAllHyojiSetteiHaba.Text = Consts.zenbyouji

                ' 設定幅情報を取得する。
                ' 以下のアクションクラスを呼び出す。
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "BtnAllHyojiSetteiHaba_Click"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                If Not IsNothing(qc001F04FormDto.HyojiHabaList) AndAlso
                           qc001F04FormDto.HyojiHabaList.Count > 0 Then
                    MeisaiHabaSetting(CommUtility.ConvertSettingDto(qc001F04FormDto.HyojiHabaList))
                End If
            End If
            ' #13489 end

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿幅記憶ボタン
        ''' </summary>
        Public Sub BtnHabaKioku_Click() Handles btnHabaKioku.Click

            '開始処理を行う
            InitProcess()

            Try
                btnHabaKioku.Enabled = False
                Dim newSettingDtoList As New List(Of MJSettingDto)
                For Each col As Column In Me.sprM1MenuIchiran_Sheet1.Columns
                    Dim SettingDto = qc001F04FormDto.HyojiHabaList.Find(Function(dto) dto.Section = Consts.habakioku AndAlso dto.Param = col.Label)
                    If IsNothing(SettingDto) Then
                        SettingDto = New MJSettingDto With {
                           .ItemDivision = "G",
                           .ItemId = QC001F04Form.FORM_ID,
                           .Remarks = String.Empty,
                           .Section = Consts.habakioku,
                           .SettingFileName = "NA",
                           .TenkaCode = "ZZZZZ",
                           .UserId = ApplicationScope.LoginInfo.SyainCode,
                           .Param = col.Label
                       }
                    End If
                    SettingDto.Value = col.Width
                    SettingDto.UpdateFlag = True
                    newSettingDtoList.Add(SettingDto)
                Next
                qc001F04FormDto.HyojiHabaList = newSettingDtoList

                ' 幅記憶情報設定アクションクラスを呼び出す
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "BtnHabaKioku_Click"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
            Finally
                btnHabaKioku.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿並び替え▲ボタンクリック
        ''' </summary>
        Private Sub btnNarabikaeUp_Click(sender As Object, e As EventArgs) Handles btnNarabikaeUp.Click

            '開始処理を行う
            InitProcess()

            ' 明細チェック:01
            '#6144
            'ST_#5729 Start 潘 2021/09/03
            Dim senFlg As Boolean = False
            ' 選択行の行番号取得
            Dim selectRowNum = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)
            '選択最後行(空白行)
            If selectRowNum.Equals(qc001F04FormDto.SprM1MenuIchiran.Count - 1) AndAlso
                String.IsNullOrWhiteSpace(qc001F04FormDto.SprM1MenuIchiran(selectRowNum).M1MenuNo) Then
                senFlg = True
            End If

            If sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow < 0 OrElse senFlg Then
                'ST_#5729 End 潘 2021/09/03
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                Exit Sub
            End If

            Me.changedFlg = False

            ' 選択行の行番号取得
            '#6144
            Dim selectRowIndex = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)
            '#4902 START
            Dim selectRowBk = selectRowIndex
            '#4902 END

            If selectRowIndex.Equals(0) Then
                Me.changedFlg = True
                Exit Sub
            End If
            ' #10554 START
            ' 選択行の取得

            If Not Me.CheckMenuChoose(qc001F04FormDto, selectRowIndex) Then
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1485))
                ' #3480 START
                Me.changedFlg = True
                ' #3480 END
                Exit Sub
            End If
            ' #10554 END
            ' 共有_たよトラン情報
            Dim qc001MJTA = SharedComClient.InstanceData.QC001_MJTA
            ' #6545 start
            ' 移動行数を取得する
            ' 移動行数
            Dim selectRowCnt As Integer
            ' 最後行の次行
            Dim selectRowEnd As Integer = selectRowIndex + 1

            Me.GetoyaRow(selectRowIndex, selectRowEnd, selectRowCnt)

            If selectRowIndex = 0 Then
                Me.changedFlg = True
                Exit Sub
            End If
            ' ターゲット行数
            Dim targetRow As Integer
            If String.Equals(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex - 1).M1Syubetu, Consts.M1Syubetu.ka) Then
                If (String.Equals(CStr(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex - 1).QC001S04MstDetailKakinDto, "OPTION_SYUBETU")), "1") AndAlso
                   String.IsNullOrEmpty(CStr(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex - 1).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO")))) Then
                    Dim oyaMenuDto = qc001F04FormDto.SprM1MenuIchiran.ToList.Find(Function(o) String.Equals(o.M1NaibuNo, qc001F04FormDto.SprM1MenuIchiran(selectRowIndex - 1).SerMenuno) AndAlso
                                                                                             String.Equals(o.CmbM1SettisakiComboCode, qc001F04FormDto.SprM1MenuIchiran(selectRowIndex - 1).CmbM1SettisakiComboCode))

                    targetRow = CInt(oyaMenuDto.M1No) - 1
                ElseIf String.Equals(CStr(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex - 1).QC001S04MstDetailKakinDto, "OPTION_SYUBETU")), "2") Then
                    Dim sonMenuDto = qc001F04FormDto.SprM1MenuIchiran.ToList.Find(Function(o) String.Equals(o.M1NaibuNo, qc001F04FormDto.SprM1MenuIchiran(selectRowIndex - 1).SerMenuno) AndAlso
                                                                                              String.Equals(o.CmbM1SettisakiComboCode, qc001F04FormDto.SprM1MenuIchiran(selectRowIndex - 1).CmbM1SettisakiComboCode))

                    If String.Equals(Me.GetValueDic(sonMenuDto.QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), "1") AndAlso
                       Not String.IsNullOrEmpty(CStr(Me.GetValueDic(sonMenuDto.QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"))) Then
                        targetRow = CInt(sonMenuDto.M1No) - 1
                    Else
                        Dim oyaMenuDto = qc001F04FormDto.SprM1MenuIchiran.ToList.Find(Function(o) String.Equals(o.M1NaibuNo, sonMenuDto.SerMenuno) AndAlso
                                                                                                  String.Equals(o.CmbM1SettisakiComboCode, sonMenuDto.CmbM1SettisakiComboCode))

                        targetRow = CInt(oyaMenuDto.M1No) - 1
                    End If
                Else
                    targetRow = selectRowIndex - 1
                End If
            Else
                targetRow = selectRowIndex - 1
            End If
            ' #6545 end
            ' 移動行の取得
            Dim moveRowList = New List(Of QC001F04M1Dto)
            For num = 0 To selectRowCnt - 1
                moveRowList.Add(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex + num))
            Next

            ' 移動
            For Each moveRow As QC001F04M1Dto In moveRowList
                qc001F04FormDto.SprM1MenuIchiran.Remove(moveRow)
            Next
            Dim rownum As Integer = 0
            For Each moveRow As QC001F04M1Dto In moveRowList
                qc001F04FormDto.SprM1MenuIchiran.Insert(targetRow + rownum, moveRow)
                rownum += 1
            Next

            '#4902 START
            txtGenzaiNoPage.Text = CStr((targetRow + selectRowBk - selectRowIndex) \ CInt(txtIchiPageNoKensuu.Text) + 1)
            Dim newPosRow = (targetRow + selectRowBk - selectRowIndex) Mod CInt(txtIchiPageNoKensuu.Text)
            Dim selectCellrange = sprM1MenuIchiran.ActiveSheet.GetSelections
            sprM1MenuIchiran.ActiveSheet.SetActiveCell(newPosRow, sprM1MenuIchiran.ActiveSheet.ActiveColumnIndex)

            For Each item In selectCellrange
                sprM1MenuIchiran.ActiveSheet.AddSelection(newPosRow, item.Column, item.RowCount, item.ColumnCount)
            Next
            '#4902 END

            '#13555 ADD START 2022/10/04 QQ)K.Umino 並び替え時に選択したセルが隠れないよう修正
            ' 選択セルの自動スクロールを設定する 並び替え上の時 = 0
            Me.ActiveCellScroll(newPosRow, 0)
            '#13555 ADD END   2022/10/04 QQ)K.Umino

            '画面表示
            Me.Paging()

            '仕様変更対応 QC001F04_見積・契約入力【たよ明細タブ】画面＿並び替え▲ボタンクリック 2022/04/14 START
            '(5)-1.並び替えた後の明細内容を、内部データ(SharedComClient)．見積・契約入力【たよ明細タブ】フォームDTO(QC001F04FormDTO)に保存する。
            SharedComClient.InstanceData.QC001F04FormDTO = qc001F04FormDto
            '仕様変更対応 QC001F04_見積・契約入力【たよ明細タブ】画面＿並び替え▲ボタンクリック 2022/04/14 END

            Me.changedFlg = True

            '終了処理を実行する
            EndProcess()

        End Sub


        ''' <summary>
        ''' 画面＿並び替え▼ボタンクリック
        ''' </summary>
        Private Sub btnNarabikaeDown_Click(sender As Object, e As EventArgs) Handles btnNarabikaeDown.Click

            '開始処理を行う
            InitProcess()

            ' 明細チェック:01
            '#6144
            'ST_#5729 Start 潘 2021/09/03
            Dim senFlg As Boolean = False
            ' 選択行の行番号取得
            Dim selectRowNum = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)
            '選択最後行(空白行)
            If selectRowNum.Equals(qc001F04FormDto.SprM1MenuIchiran.Count - 1) AndAlso
                String.IsNullOrWhiteSpace(qc001F04FormDto.SprM1MenuIchiran(selectRowNum).M1MenuNo) Then
                senFlg = True
            End If

            If sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow < 0 OrElse senFlg Then
                'ST_#5729 End 潘 2021/09/03
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                Exit Sub
            End If

            Me.changedFlg = False

            ' 選択行の行番号取得
            '#6144
            Dim selectRowIndex = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)
            '#4902 START
            Dim selectRowBk = selectRowIndex
            '#4902 END

            If selectRowIndex.Equals(qc001F04FormDto.SprM1MenuIchiran.Count - 1) Then
                Me.changedFlg = True
                Exit Sub
            End If
            ' #10554 START
            ' 選択行の取得
            If Not Me.CheckMenuChoose(qc001F04FormDto, selectRowIndex) Then
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1485))
                ' #3480 START
                Me.changedFlg = True
                ' #3480 END
                Exit Sub

            End If
            ' #10554 END
            ' 共有_たよトラン情報
            Dim qc001MJTA = SharedComClient.InstanceData.QC001_MJTA
            ' #6545 start
            ' 移動行数を取得する
            ' 移動行数
            Dim selectRowCnt As Integer
            ' 最後行の次行
            Dim selectRowEnd As Integer = selectRowIndex + 1
            Me.GetoyaRow(selectRowIndex, selectRowEnd, selectRowCnt)

            If qc001F04FormDto.SprM1MenuIchiran.Count - 1 < selectRowEnd + 1 Then
                Me.changedFlg = True
                Exit Sub
            End If

            ' ターゲット行数
            Dim targetRow As Integer
            If String.Equals(qc001F04FormDto.SprM1MenuIchiran(selectRowEnd).M1Syubetu, Consts.M1Syubetu.ka) Then
                If String.Equals(CStr(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowEnd).QC001S04MstDetailKakinDto, "OPTION_SYUBETU")), "0") OrElse
                    (String.Equals(CStr(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowEnd).QC001S04MstDetailKakinDto, "OPTION_SYUBETU")), "1" AndAlso
                       String.IsNullOrEmpty(CStr(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowEnd).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"))))) Then
                    Dim selectRowLastIndex As Integer = selectRowEnd
                    Dim selectLastRowEnd As Integer = selectRowLastIndex + 1
                    Dim selectLastRowCnt As Integer = 0
                    Me.GetoyaRow(selectRowLastIndex, selectLastRowEnd, selectLastRowCnt)
                    targetRow = selectLastRowEnd
                Else
                    targetRow = selectRowEnd + 1
                End If
            Else
                targetRow = selectRowEnd + 1
            End If
            ' #6545 end
            ' 移動行の取得
            Dim moveRowList = New List(Of QC001F04M1Dto)
            For num = 0 To selectRowCnt - 1
                moveRowList.Add(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex + num))
            Next
            ' 移動
            Dim rownum As Integer = 0
            For Each moveRow As QC001F04M1Dto In moveRowList
                Dim copyMoveRow = CommUtility.ObjectClone(moveRow)
                qc001F04FormDto.SprM1MenuIchiran.Insert(targetRow + rownum, copyMoveRow)
                rownum += 1
            Next
            For Each moveRow As QC001F04M1Dto In moveRowList
                qc001F04FormDto.SprM1MenuIchiran.Remove(moveRow)
            Next

            '#4902 START
            txtGenzaiNoPage.Text = CStr((targetRow - selectRowCnt + selectRowBk - selectRowIndex) \ CInt(txtIchiPageNoKensuu.Text) + 1)
            Dim newPosRow = (targetRow - selectRowCnt + selectRowBk - selectRowIndex) Mod CInt(txtIchiPageNoKensuu.Text)
            Dim selectCellrange = sprM1MenuIchiran.ActiveSheet.GetSelections
            sprM1MenuIchiran.ActiveSheet.SetActiveCell(newPosRow, sprM1MenuIchiran.ActiveSheet.ActiveColumnIndex)

            For Each item In selectCellrange
                sprM1MenuIchiran.ActiveSheet.AddSelection(newPosRow, item.Column, item.RowCount, item.ColumnCount)
            Next
            '#4902 END

            '#13555 ADD START 2022/10/04 QQ)K.Umino 並び替え時に選択したセルが隠れないよう修正
            ' 選択セルの自動スクロールを設定する 並び替え下の時 = 1
            Me.ActiveCellScroll(newPosRow, 1)
            '#13555 ADD END   2022/10/04 QQ)K.Umino

            '画面表示
            Me.Paging()

            '仕様変更対応 QC001F04_見積・契約入力【たよ明細タブ】画面＿並び替え▼ボタンクリック 2022/04/14 START
            '(5)-1.並び替えた後の明細内容を、内部データ(SharedComClient)．見積・契約入力【たよ明細タブ】フォームDTO(QC001F04FormDTO)に保存する。
            SharedComClient.InstanceData.QC001F04FormDTO = qc001F04FormDto
            '仕様変更対応 QC001F04_見積・契約入力【たよ明細タブ】画面＿並び替え▼ボタンクリック 2022/04/14 END

            Me.changedFlg = True

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 右クリック：複写
        ''' </summary>
        Private Sub rightClickCopy_Click(sender As Object, e As EventArgs) Handles RightClickCopy.Click

            '開始処理を行う
            InitProcess()

            Dim menu As Windows.Forms.ToolStripMenuItem = CType(sender, Windows.Forms.ToolStripMenuItem)
            Try
                menu.Enabled = False

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "RightClickCopy_Click"

                ' 選択行がない場合
                ''#4491
                '#6472 start
                'If sprM1MenuIchiran_Sheet1.GetSelections.Count = 0 Then
                If Not sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow >= 0 Then
                    ' 明細チェック:01 「メニュー明細エリア」で１行も選択されていない場合エラー
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                    '#6472 end
                    Return
                End If

                ' 選択行の取得
                Me.GetRowIndex()

                If qc001F04FormDto.SelectedRowIndex.Count = 0 Then
                    Exit Sub
                End If

                '8208 Start
                If Not GetOyakoMenuChoose() Then
                    Exit Sub
                End If
                '8208 End

                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
            Finally
                menu.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　画面＿複写ボタンクリック　Start
        ''' <summary>
        ''' 複写
        ''' </summary>
        Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
            ClientLogUtil.Logger.DebugAP("QC001F04Form:btnCopy_Click start")
            Me.RightClickCopy.PerformClick()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:btnCopy_Click end")
        End Sub
        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　画面＿複写ボタンクリック　End

        ''' <summary>
        ''' 右クリック：一括グループ設定
        ''' </summary>
        Private Sub ToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click

            '開始処理を行う
            InitProcess()

            Dim menu As Windows.Forms.ToolStripMenuItem = CType(sender, Windows.Forms.ToolStripMenuItem)
            Try
                menu.Enabled = False
                If Not Me.changedFlg Then
                    Exit Sub
                End If

                ' 明細チェック:01 「メニュー明細エリア」で１行も選択されていない場合エラー
                ''#4491
                If sprM1MenuIchiran_Sheet1.GetSelections.Count = 0 Then
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                    Exit Sub
                End If

                ' 選択行の取得
                Me.GetRowIndex()

                If qc001F04FormDto.SelectedRowIndex.Count = 0 OrElse
                    (qc001F04FormDto.SelectedRowIndex.Count = 1 AndAlso String.IsNullOrEmpty(qc001F04FormDto.SprM1MenuIchiran(qc001F04FormDto.SelectedRowIndex(0)).M1NaibuNo)) Then
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                    Exit Sub
                End If

                Dim startnum As Integer = qc001F04FormDto.SelectedRowIndex(0)
                '#4491 EK1436削除
                'For Each num As Integer In qc001F04FormDto.SelectedRowIndex
                '    If Not String.Equals(num, startnum) Then
                '        MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1436))
                '        Exit Sub
                '    End If
                '    startnum += 1
                'Next

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "ToolStripMenuItem3_Click"

                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                If Not qc001F04FormDto.ContinueFlg Then
                    qc001F04FormDto.SprM1MenuIchiran = CType(Copy(qc001F04FormDto.SprM1MenuIchiranBk), BindingList(Of QC001F04M1Dto))
                    qc001F04FormDto.ContinueFlg = True
                    Return
                End If

                '画面表示
                Me.Paging()
            Finally
                menu.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 右クリック：貼り付け時
        ''' </summary>
        Private Sub rightClickPaste_Click(sender As Object, e As EventArgs) Handles RightClickCopyPaste.Click

            '開始処理を行う
            InitProcess()

            Dim menu As Windows.Forms.ToolStripMenuItem = CType(sender, Windows.Forms.ToolStripMenuItem)
            Try
                menu.Enabled = False
                If Not Me.changedFlg Then
                    Exit Sub
                End If

                Me.changedFlg = False

                ' 選択行がない場合
                ''#4491
                '#6472 start
                'If sprM1MenuIchiran_Sheet1.GetSelections.Count = 0 Then
                If Not sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow >= 0 Then
                    '#6472 end
                    ' #3480 START
                    Me.changedFlg = True
                    ' #3480 END
                    Return
                End If

                '2022/08/18 #12687 DEL-START メニュー切り取り表示不正対応
                '8208 Start
                'Dim selectRowList = qc001F04FormDto.SelectedRowIndex
                '8208 End
                '2022/08/18 #12687 #DEL-END

                ' 選択行の取得
                Me.GetRowIndex()

                If qc001F04FormDto.SelectedRowIndex.Count = 0 Then
                    Exit Sub
                End If

                'クリップボードに複写内容が存在する場合
                Dim copyData = Clipboard.GetData(Consts.HaritukeokonauCopy)
                If copyData IsNot Nothing Then
                    qc001F04FormDto.CopyRowList = CType(copyData, List(Of QC001F04M1Dto))

                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "RightClickCopyPaste_Click"

                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                End If

                ' クリップボードに切取内容が存在する場合
                Dim cutData = Clipboard.GetData(Consts.HaritukeokonauCut)
                If cutData IsNot Nothing Then

                    '2022/08/18 #12687 DEL-START メニュー切り取り表示不正対応
                    '8208 Start
                    ' 選択行の削除
                    'Dim deleteRowList As New List(Of QC001F04M1Dto)
                    'For Each rowNo In selectRowList
                    '    deleteRowList.Add(qc001F04FormDto.SprM1MenuIchiran(rowNo))
                    '    If qc001F04FormDto.SelectedRowIndex(0) > rowNo Then
                    '        qc001F04FormDto.SelectedRowIndex(0) = qc001F04FormDto.SelectedRowIndex(0) - 1
                    '    End If
                    'Next
                    'For Each delRow In deleteRowList
                    '    qc001F04FormDto.SprM1MenuIchiran.Remove(delRow)
                    'Next

                    '画面表示
                    'Me.Paging()
                    '8208 End
                    '2022/08/18 #12687 #DEL-END

                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "RightClickCutPaste_Click"

                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                End If

                ' クリップボードにメニュー番号が存在する場合
                Dim textData = Clipboard.GetData(DataFormats.Text)
                If textData IsNot Nothing Then
                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                    Dim actionMethodName As String = "RightClickPaste_Click"

                    ' QC001F00Formのアドオンチェックボタン制御
                    Me.CallQC001F00FormAddonCheckSeigyo()

                    ' クリップボード内容取得
                    sprM1MenuIchiran_Sheet1.Cells(qc001F04FormDto.SelectedRowIndex(0), 1).Value = textData

                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                End If

                ' 設置先コンボ編集
                Me.SetteiSettisakiCombo()

                Me.CallQC001F00FormAddonCheckSeigyo()

                '画面表示
                Me.Paging()

                Me.changedFlg = True
            Finally
                menu.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　画面＿貼り付けボタンクリック　Start
        ''' <summary>
        ''' 貼り付け時
        ''' </summary>
        Private Sub btnPaste_Click(sender As Object, e As EventArgs) Handles btnPaste.Click
            ClientLogUtil.Logger.DebugAP("QC001F04Form:btnPaste_Click start")
            Me.RightClickCopyPaste.PerformClick()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:btnPaste_Click end")
        End Sub
        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　画面＿貼り付けボタンクリック　End

        ''' <summary>
        ''' 右クリック：切取
        ''' </summary>
        Private Sub rightClickCut_Click(sender As Object, e As EventArgs) Handles RightClickCut.Click

            '開始処理を行う
            InitProcess()

            Dim menu As Windows.Forms.ToolStripMenuItem = CType(sender, Windows.Forms.ToolStripMenuItem)
            Try
                menu.Enabled = False

                Me.changedFlg = False

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "RightClickCut_Click"

                ' 選択行がない場合
                ''#4491
                '#6472 start
                'If sprM1MenuIchiran_Sheet1.GetSelections.Count = 0 Then
                If Not sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow >= 0 Then
                    '#6472 end
                    Return
                End If

                ' 選択行の取得
                Me.GetRowIndex()

                If qc001F04FormDto.SelectedRowIndex.Count = 0 Then
                    Exit Sub
                End If

                '8208 Start
                If Not GetOyakoMenuChoose() Then
                    Exit Sub
                End If
                '8208 End

                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                '8208 Start
                '' 選択行の削除
                'Dim deleteRowList As New List(Of QC001F04M1Dto)
                'For Each rowNo In qc001F04FormDto.SelectedRowIndex
                '    deleteRowList.Add(qc001F04FormDto.SprM1MenuIchiran(rowNo))
                'Next
                'For Each delRow In deleteRowList
                '    qc001F04FormDto.SprM1MenuIchiran.Remove(delRow)
                'Next

                ''画面表示
                'Me.Paging()
                '8208 End

                Me.changedFlg = True
            Finally
                menu.Enabled = True
            End Try

            '#12691 2022/09/05 Start
            Me.CallQC001F00FormAddonCheckSeigyo()

            '#6472 start
            Me.Paging()
            '#6472 end

            '終了処理を実行する
            EndProcess()

        End Sub

        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　画面＿切取ボタンクリック　Start
        ''' <summary>
        ''' 切取
        ''' </summary>
        Private Sub btnCut_Click(sender As Object, e As EventArgs) Handles btnCut.Click
            ClientLogUtil.Logger.DebugAP("QC001F04Form:btnCut_Click start")
            Me.RightClickCut.PerformClick()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:btnCut_Click end")
        End Sub
        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　画面＿切取ボタンクリック　End

        ''' <summary>
        ''' 右クリック：行挿入
        ''' </summary>
        Private Sub rightClickInsert_Click(sender As Object, e As EventArgs) Handles RightClickInsert.Click

            '開始処理を行う
            InitProcess()

            ' 明細チェック:01
            ''#4491
            '#6472 start
            'If sprM1MenuIchiran_Sheet1.GetSelections.Count = 0 Then
            If Not sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow >= 0 Then
                '#6472 end
                MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                Exit Sub
            End If

            Me.changedFlg = False

            ' 選択行の取得
            ''#4491
            '#6472 start
            'Dim selectRowIndex = (CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + sprM1MenuIchiran_Sheet1.GetSelections(0).Row
            Me.GetRowIndex()
            '#6472 end

            '' 選択行の行数を取得する
            ''#4491
            '#6472 start
            'Dim selectRowCnt = sprM1MenuIchiran_Sheet1.GetSelections(0).RowCount
            '#6472 end

            '仕様変更対応 QC001F04_見積・契約入力【たよ明細タブ】右クリック：行挿入 2022/04/14 START
            Dim newIndex As Integer
            '#6472 start
            Dim rowIndex = sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow
            Dim selectFirstRowIndex As Integer = getRowNo(rowIndex)
            If selectFirstRowIndex = 0 Then
                '先頭行に対し、行挿入を行う場合
                newIndex = selectFirstRowIndex
            Else
                '先頭行以外の行に対し、行挿入を行う場合
                newIndex = selectFirstRowIndex - 1
            End If
            '#6472 end


            ' 行挿入
            ''#4491
            '#6472 start
            'For i As Integer = 1 To sprM1MenuIchiran_Sheet1.GetSelections.Count
            For i As Integer = 1 To qc001F04FormDto.SelectedRowIndex.Count Step 1
                Dim M1Dto As New QC001F04M1Dto With {
                   .M1No = qc001F04FormDto.SelectedRowIndex.Count + 1 - i,
                   .CmbM1SettisakiCombo = qc001F04FormDto.SprM1MenuIchiran(newIndex).CmbM1SettisakiCombo,
                   .CmbM1SettisakiComboChoose = qc001F04FormDto.SprM1MenuIchiran(newIndex).CmbM1SettisakiComboChoose,
                   .CmbM1SettisakiComboCode = qc001F04FormDto.SprM1MenuIchiran(newIndex).CmbM1SettisakiComboCode,
                   .CmbM1GroupCombo = qc001F04FormDto.SprM1MenuIchiran(newIndex).CmbM1GroupCombo,
                   .CmbM1GroupComboChoose = qc001F04FormDto.SprM1MenuIchiran(newIndex).CmbM1GroupComboChoose,
                   .CmbM1GroupComboCode = qc001F04FormDto.SprM1MenuIchiran(newIndex).CmbM1GroupComboCode
                }
                '.M1No = sprM1MenuIchiran_Sheet1.GetSelections.Count + 1 - i,
                qc001F04FormDto.SprM1MenuIchiran.Insert(selectFirstRowIndex, M1Dto)
                '#6472 end
            Next
            '仕様変更対応 QC001F04_見積・契約入力【たよ明細タブ】右クリック：行挿入 2022/04/14 END
            ' 画面表示
            Me.Paging()
            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()

            Me.changedFlg = True

            '終了処理を実行する
            EndProcess()

        End Sub

        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　画面＿行挿入ボタンクリック　Start
        ''' <summary>
        ''' 行挿入
        ''' </summary>
        Private Sub btnInsert_Click(sender As Object, e As EventArgs) Handles btnRowInsert.Click
            ClientLogUtil.Logger.DebugAP("QC001F04Form:btnInsert_Click start")
            Me.RightClickInsert.PerformClick()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:btnInsert_Click end")
        End Sub
        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　画面＿行挿入ボタンクリック　End

        ''' <summary>
        ''' 右クリック：削除
        ''' </summary>
        Private Sub rightClickDelete_Click(sender As Object, e As EventArgs) Handles RightClickDelete.Click

            '開始処理を行う
            InitProcess()

            Dim menu As Windows.Forms.ToolStripMenuItem = CType(sender, Windows.Forms.ToolStripMenuItem)
            Try
                menu.Enabled = False
                ' 明細チェック:01
                '#4491
                'ST_#5744 Start 潘 2021/09/03
                Dim senFlg As Boolean = False
                ' 選択行の行番号取得
                Dim selectRowNum = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)
                ''選択最後行(空白行)
                If selectRowNum.Equals(qc001F04FormDto.SprM1MenuIchiran.Count - 1) AndAlso
                   String.IsNullOrWhiteSpace(qc001F04FormDto.SprM1MenuIchiran(selectRowNum).M1MenuNo) Then
                    senFlg = True
                End If
                If String.Equals(sprM1MenuIchiran_Sheet1.GetSelections.Length, 0) OrElse
                    sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow < 0 OrElse
                    senFlg Then
                    'ST_#5744 End 潘 2021/09/03
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK0710))
                    Exit Sub
                End If
                '#4902 START
                ' 選択行の取得
                Me.GetRowIndex()

                Dim selectRowList As New List(Of Integer)
                For Each itemRowIndex In qc001F04FormDto.SelectedRowIndex
                    'ST_#5744 Start 潘 2021/09/03
                    If selectRowList.Contains(itemRowIndex) OrElse itemRowIndex < 0 Then
                        'ST_#5744 End 潘 2021/09/03
                        Continue For
                    End If
                    Dim selectRowIndex As Integer = itemRowIndex
                    ' 移動行数
                    Dim selectRowCnt As Integer
                    ' 最後行の次行
                    Dim selectRowEnd As Integer = selectRowIndex + 1

                    '2022.09.27 MOD-START #13679
                    'メニューに紐づく行選択を行う。行削除のみ異なるルートを使用する
                    'Me.GetoyaRow(selectRowIndex, selectRowEnd, selectRowCnt)
                    Me.GetoyaRow(selectRowIndex, selectRowEnd, selectRowCnt, "Delete")
                    '2022.09.27 MOD-END #13679

                    '#6154 START
                    ''選択行設定
                    'For num = 0 To selectRowCnt - 1
                    '    selectRowList.Add(selectRowIndex + num)
                    'Next
                    '#6154 END

                    '2022.07.17  FNST)zlx ADD-START #10570
                    For num = 0 To selectRowCnt - 1
                        qc001F04FormDto.SprM1MenuIchiran(selectRowIndex + num).ShinMenuflag = False
                    Next
                    '2022.07.17  FNST)zlx ADD-END #10570
                    If selectRowCnt > 1 Then
                        '親の場合
                        If selectRowIndex = itemRowIndex Then
                            For num = selectRowIndex To selectRowIndex + selectRowCnt - 1
                                If Not qc001F04FormDto.SelectedRowIndex.Contains(num) Then
                                    Dim messageChooose = MessageDialogUtil.ShowWarn(MessageUtil.GetDialogProperty(BusinessMessageConst.WK8440,
                                                                              qc001F04FormDto.SprM1MenuIchiran(itemRowIndex).M1MenuNo,
                                                                              qc001F04FormDto.SprM1MenuIchiran(itemRowIndex).M1MenuNm))
                                    If Not String.Equals(messageChooose, Consts.MessageSelect.Hai) Then
                                        Exit Sub
                                    End If
                                    Exit For
                                End If
                            Next

                            '#6154 START
                            '選択行設定
                            '2022.07.17  FNST)zlx ADD-START #10570
                            Dim M1Menu_Seq As String = qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).M1MenuSeq
                            '2022.07.17  FNST)zlx ADD-END #10570
                            For num = 0 To selectRowCnt - 1
                                '2022.07.17  FNST)zlx UPD-START #10570
                                If String.Equals(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex + num).M1MenuSeq, M1Menu_Seq) Then
                                    selectRowList.Add(selectRowIndex + num)
                                    '2022.09.27 MOD-START #13679
                                    'qc001F04FormDto.SprM1MenuIchiran(selectRowIndex + num).ShinMenuflag = True
                                    qc001F04FormDto.SprM1MenuIchiran(itemRowIndex).ShinMenuflag = True
                                    '2022.09.27 MOD-END #13679
                                End If
                                'selectRowList.Add(selectRowIndex + num)
                                '2022.07.17  FNST)zlx UPD-END #10570
                            Next
                            '#6154 END
                        Else
                            '#6154 START
                            'If Not qc001F04FormDto.SelectedRowIndex.Contains(selectRowIndex) Then
                            '    Dim messageChooose = MessageDialogUtil.ShowWarn(MessageUtil.GetDialogProperty(BusinessMessageConst.WK8439,
                            '                                              qc001F04FormDto.SprM1MenuIchiran(itemRowIndex).M1MenuNo,
                            '                                              qc001F04FormDto.SprM1MenuIchiran(itemRowIndex).M1MenuNm))
                            '    If Not String.Equals(messageChooose, Consts.MessageSelect.Hai) Then
                            '        Exit Sub
                            '    End If
                            'End If
                            '#12202 2022.08.03 START
                            'selectRowList.Add(itemRowIndex)
                            '#6154 END
                            Dim M1Menu_Seq As String = qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).M1MenuSeq
                            For num = 0 To selectRowCnt - 1
                                If String.Equals(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex + num).M1MenuSeq, M1Menu_Seq) Then
                                    selectRowList.Add(selectRowIndex + num)
                                    '2022.09.27 MOD-START #13679
                                    'qc001F04FormDto.SprM1MenuIchiran(selectRowIndex + num).ShinMenuflag = True
                                    qc001F04FormDto.SprM1MenuIchiran(itemRowIndex).ShinMenuflag = True
                                    '2022.09.27 MOD-END #13679
                                End If
                            Next
                            '#12202 2022.08.03 END
                        End If
                        ' ST#5744 START
                    Else
                        selectRowList.Add(itemRowIndex)
                        ' ST#5744 END
                    End If
                Next

                qc001F04FormDto.SelectedRowIndex = selectRowList

                Me.changedFlg = False
                '#4902 END

                If qc001F04FormDto.SelectedRowIndex.Count = 0 Then
                    Exit Sub
                End If

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "rightClickDelete_Click"
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                If qc001F04FormDto.ContinueFlg Then

                    ' 画面表示
                    Me.Paging()

                    ' QC001F00Formのアドオンチェックボタン制御
                    Me.CallQC001F00FormAddonCheckSeigyo()
                Else

                    qc001F04FormDto.ContinueFlg = True
                End If
                Me.changedFlg = True
            Finally
                menu.Enabled = True
            End Try

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 画面＿値引設定_丸め設定コンボロストフォーカス
        ''' </summary>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub cmbNebikiSetteiMarumeSetteiCombo_LostFocus(sender As Object, e As EventArgs) Handles cmbNebikiSetteiMarumeSetteiCombo.SelectedIndexChanged
        Private Sub cmbNebikiSetteiMarumeSetteiCombo_LostFocus(sender As Object, e As EventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            If Not Me.changedFlg Then
                Exit Sub
            End If

            Me.BindFormToDto()
            If CType(Me.cmbNebikiSetteiMarumeSetteiCombo.SelectedValue, String) <> qc001F04FormDto.CmbNebikiSetteiMarumeSettei Then

                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "CmbNebikiSetteiMarumeSetteiCombo_LostFocus"
                Dim buttonId As String = Consts.blank
                UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)

                '画面表示
                Me.Paging()
            End If

        End Sub

        ''' <summary>
        ''' 納地・設置ボタンクリック時処理
        ''' </summary>
        Public Sub BtnNochiSettiClick_Click()
            Me.changedFlg = False
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnNochiSettiClick_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()
            '画面表示
            Me.Paging()

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()
            Me.changedFlg = True
        End Sub

        ''' <summary>
        ''' 明細引用ボタンクリック時処理
        ''' </summary>
        Public Sub BtnMeisaiInyo_Click()

            UpdateProcessingFlagToFalse()

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnMeisaiInyo_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
            '画面表示
            Me.Paging()
            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()

        End Sub

        ''' <summary>
        ''' アドオンチェックボタン時処理
        ''' </summary>
        Public Sub BtnAddonCheck_Click()
            Me.changedFlg = False
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnAddonCheck_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)

            ' 設置先コンボ編集
            Me.SetteiSettisakiCombo()

            '2022/10/18 MOD-START #14079　選択メニュー画面が起動しない為に
            '画面表示
            If Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                Me.Paging()
            End If
            '2022/10/18 MOD-END #14079

            ' QC001F00Formのアドオンチェックボタン制御
            Me.CallQC001F00FormAddonCheckSeigyo()
            Me.changedFlg = True
        End Sub

        ''' <summary>
        ''' 見積設定ボタンクリック時処理
        ''' </summary>
        Public Sub BtnMitsumoriSettei_Click()

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnMitsumoriSettei_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)

            'ST1_#5963 START
            Paging()
            'ST1_#5963 END
        End Sub

        ''' <summary>
        ''' 設定－パーソナル設定クリック時処理
        ''' </summary>
        Public Sub MenuSetteiPersonalSettei_Click()

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "MenuSetteiPersonalSettei_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
            MeisaiHabaSetting(CommUtility.ConvertSettingDto(qc001F04FormDto.HyojiHabaList))

        End Sub

        ''' <summary>
        ''' 設定－取込設定メニュークリック時処理
        ''' </summary>
        Public Sub MenuSetteiTorikomiSettei_Click()

            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "MenuSetteiTorikomiSettei_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
            Me.Paging()
        End Sub

        ''' <summary>
        ''' データ書込ボタンクリック時処理
        ''' </summary>
        Public Function BtnDataKakikomi_Click() As Boolean
            ClientLogUtil.Logger.DebugAP("QC001F04Form:BtnDataKakikomi_Click start")
            'ST先行検証 #10337  ADD START
            ' 継続フラグの初期化
            qc001F04FormDto.ContinueFlg = True
            'ST先行検証 #10337  ADD END
            ' 障害_ST先行検証 #10558 ADD START
            ' 補正のタイミング -「データ書込」ボタン押下時
            ' 参照モード以外の場合
            If Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                ' 値増しチェック
                Dim actionNameF04 As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodNameF04 As String = "NemashiCheck"
                UpdateProcessingFlagToFalse()
                ExecuteAction(actionNameF04, actionMethodNameF04, qc001F04FormDto, Nothing)
            End If
            ' 障害_ST先行検証 #10558 ADD END

            qc001F04FormDto.ShoriKbn = "01"
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnDataKakikomi_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)

            ClientLogUtil.Logger.DebugAP("QC001F04Form:BtnDataKakikomi_Click end")

            If Not qc001F04FormDto.ContinueFlg Then
                qc001F04FormDto.ContinueFlg = True
                Return False
            Else
                Return True
            End If

        End Function

        ''' <summary>
        ''' 他タブへ遷移
        ''' </summary>
        Public Sub Tab_Leave()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:Tab_Leave start")

            '#9288 20221028 張Inc）邊 START
            Dim action As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethod As String = "EditTransactionOnly"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            qc001F04FormDto = CType(ExecuteAction(action, actionMethod, qc001F04FormDto, Nothing), QC001F04FormDto)
            '#9288 20221028 張Inc）邊 END

            '(5)-1料金再計算の要否を判断
            '①内部_保守ヘッダファイルDTO. 再計算フラグ = "1"  (データの再計算が必要)　かつ
            '②保守メニューマスタ.ソフトセット区分="4"(ｿﾌﾄ積上げ)、且つ 保守ソフトファイル.保守金額=0（MJTASFT.HOSKIN=0）
            ' 共有_たよトラン情報
            Dim qc001Mjta = CType(SharedComClient.InstanceData.GetKidoParameter(Consts.QC001_Tayo), QC001_MJTA)
            Dim sakeisanFlag = False
            For i = 0 To qc001F04FormDto.SprM1MenuIchiran.Count - 1
                If Not IsNothing(qc001Mjta) AndAlso
                   Not IsNothing(qc001Mjta.QC001_MJTAHEDDTOList) AndAlso
                   qc001Mjta.QC001_MJTAHEDDTOList.Count > 0 AndAlso
                   Not IsNothing(qc001F04FormDto.SprM1MenuIchiran(i).QC001MJTAMNUDto) AndAlso
                   Not IsNothing(qc001F04FormDto.SprM1MenuIchiran(i).QC001MJTAMNUDto.MJTAMNUDTO) Then
                    Dim mjtahedDto = qc001Mjta.QC001_MJTAHEDDTOList.Find(Function(o) String.Equals(o.MJTAHEDDTO.KEY_MJ, qc001F04FormDto.SprM1MenuIchiran(i).QC001MJTAMNUDto.MJTAMNUDTO.KEY_MJ) AndAlso
                                                            String.Equals(o.MJTAHEDDTO.KEY_MJ_HAN, qc001F04FormDto.SprM1MenuIchiran(i).QC001MJTAMNUDto.MJTAMNUDTO.KEY_MJ_HAN) AndAlso
                                                                    String.Equals(o.MJTAHEDDTO.KEY_GEN, qc001F04FormDto.SprM1MenuIchiran(i).QC001MJTAMNUDto.MJTAMNUDTO.KEY_GEN) AndAlso
                                                                    String.Equals(o.MJTAHEDDTO.KEY_EDA, qc001F04FormDto.SprM1MenuIchiran(i).QC001MJTAMNUDto.MJTAMNUDTO.KEY_EDA))
                    If Not IsNothing(mjtahedDto) Then
                        If String.Equals("1", mjtahedDto.MJTAHEDDTO.SAIKEISAN_FLG) Then
                            If {"4"}.Contains(GetValueDic(qc001F04FormDto.SprM1MenuIchiran(i).QC001S04MstDetailComDto, "SSETKBN")) Then
                                For Each mjtasftDto In qc001F04FormDto.SprM1MenuIchiran(i).QC001MJTAMNUDto.QC001_MJTASFTDTOList
                                    If Not CodeConst.KBN_0497_3.Equals(mjtasftDto.MJTASFTDTO.STATUS) AndAlso
                                        mjtasftDto.MJTASFTDTO.HOSKIN = 0 Then
                                        sakeisanFlag = True
                                        If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnSyoriMode, Consts.SyoriMode.Change) Then
                                            If Not String.Equals(qc001F04FormDto.SprM1MenuIchiran(i).Status, "3") Then
                                                sprM1MenuIchiran.ActiveSheet.AddSelection(i, 0, 1, sprM1MenuIchiran.ActiveSheet.ColumnCount - 1)
                                            End If
                                        Else
                                            sprM1MenuIchiran.ActiveSheet.AddSelection(i, 0, 1, sprM1MenuIchiran.ActiveSheet.ColumnCount - 1)
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            Next
            If sakeisanFlag Then
                Me.btnRyokinSaiKeisan.PerformClick()
            End If

            '20220812 ST#12615 ADD-START
            Me.GetRowIndex()
            '20220812 ST#12615 ADD-END
            Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
            Dim actionMethodName As String = "BtnTayoMeisai_Click"
            UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
            ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing)
            '2022.10.28  FNST) ADD-START #5226
            Dim checkFlg04 = SharedComClient.InstanceData.QC001F04FormDTO.ContinueFlg
            '2022.10.28  FNST) ADD-END #5226
            '#13586 DEL START 2022/09/29 QQ)K.Umino WK8248メッセージに対してはいを押下したとき、画面遷移を行わないよう修正
            ''10775 障害_ST先行検証 20220826 Start
            'Dim checkFlg04 = SharedComClient.InstanceData.QC001F04FormDTO.ContinueFlg
            ''10775 障害_ST先行検証 20220826 End
            '#13586 DEL END   2022/09/29 QQ)K.Umino
            ' フォームＤＴＯを退避
            SharedComClient.InstanceData.QC001F04FormDTO = Me.qc001F04FormDto
            '2022.10.28  FNST) ADD-START #5226
            SharedComClient.InstanceData.QC001F04FormDTO.ContinueFlg = checkFlg04
            '2022.10.28  FNST) ADD-END #5226
            '#13586 DEL START 2022/09/29 QQ)K.Umino WK8248メッセージに対してはいを押下したとき、画面遷移を行わないよう修正
            ''10775 障害_ST先行検証 20220826 Start
            'SharedComClient.InstanceData.QC001F04FormDTO.ContinueFlg = checkFlg04
            ''10775 障害_ST先行検証 20220826 End
            '#13586 DEL END   2022/09/29 QQ)K.Umino 
            '#11628　20220928　Start
            If Me.qc001F04FormDto.SprM1MenuIchiran.Count > 0 Then
                '#14144 20221018 statr
                'If SharedComClient.InstanceData.QC001F00FormDTO.HdnSyoriMode.Equals(Consts.SyoriMode.NewAdd) AndAlso
                '   String.IsNullOrEmpty(SharedComClient.InstanceData.QC001F00FormDTO.TogoNo) Then
                '    If SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO IsNot Nothing Then
                '        '11977 2022/10/17 START
                '        'SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO.BUPPAN_PHASE_KBN = 0
                '        SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO.TAYO_PHASE_KBN = 0
                '        '11977 2022/10/17 END
                '    End If
                'End If
                '#14144 20221018 end
                If Me.qc001F04FormDto.TayoMituChangeFlg = True Then
                    If SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO IsNot Nothing Then
                        '11977 2022/10/17 START
                        'SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO.BUPPAN_PHASE_KBN = 2
                        SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO.TAYO_PHASE_KBN = 2
                        '11977 2022/10/17 END
                    End If
                End If
                '#14144 20221018 statr
                '#11628 ST障害対応 追加 陸ウテイ 20221007 start
                'Else
                '    ' #14096 start
                '    '#11628 追加対応 修正 陸ウテイ 20221014 start
                '    'If SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO IsNot Nothing AndAlso
                '    If SharedComClient.InstanceData.QC001_MJ_TTOGOMT IsNot Nothing AndAlso
                '        SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO IsNot Nothing AndAlso
                '            SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO IsNot Nothing Then
                '        ' #14096 end
                '        'SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO.BUPPAN_PHASE_KBN = Nothing
                '        '11977 2022/10/17 START
                '        'SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO.BUPPAN_PHASE_KBN = "0"
                '        SharedComClient.InstanceData.QC001_MJ_TTOGOMT.QC001_MJ_TTOGOMTUHEADDTO.MJ_TTOGOMTUHEADDTO.TAYO_PHASE_KBN = "0"
                '        '11977 2022/10/17 END
                '        '#11628 追加対応 修正 陸ウテイ 20221014 start
                '    End If
                '#11628 ST障害対応 追加 陸ウテイ 20221007 end
                '#14144 20221018 end
            End If
            '#11628　20220928　Start

            '共通領域のクライアントログの出力
            'SharedComClient.InstanceData.QC001_DebugLog(Me.FORM_NAME, "他タブ遷移時", "")
            ClientLogUtil.Logger.DebugAP("QC001F04Form:Tab_Leave end")
        End Sub

        ''' <summary>
        ''' 固定Spreadサイズ
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub Spread_Resize(sender As Object, e As EventArgs) Handles sprHoshuRyokinSansyutsuKijunDate.Resize
        Private Sub Spread_Resize(sender As Object, e As EventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            Me.sprHoshuRyokinSansyutsuKijunDate.Size = New System.Drawing.Size(88, 24)

        End Sub

#End Region

#Region "内部処理"
        ''' <summary>
        ''' 画面項目編集
        ''' </summary>
        Private Sub EditGamenKomoku()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:EditGamenKomoku start")
            Me.changedFlg = False

            ' 保守料金算出基準日
            Me.sprHoshuRyokinSansyutsuKijunDate.Value = CommUtility.ParseDate(qc001F04FormDto.SprHoshuRyokinSansyutsuKijunDate, "yyyyMMdd")

            ' 保守区分コンボ
            Me.cmbHoshuKbnCombo.DataSource = qc001F04FormDto.CmbHoshuKbnCombo
            Me.cmbHoshuKbnCombo.ValueMember = "Code"
            Me.cmbHoshuKbnCombo.DisplayMember = "Name"
            If Not String.IsNullOrEmpty(qc001F04FormDto.CmbHoshuKbn) Then
                Me.cmbHoshuKbnCombo.SelectedValue = qc001F04FormDto.CmbHoshuKbn
            End If

            '#3450 Start
            ComboBoxWidth(cmbHoshuKbnCombo)
            '#3450 End

            If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Change) Then
                ' 契約識別２
                Me.txtKeiyakuShikibetsu2.Text = qc001F04FormDto.TxtKeiyakuShikibetsu2
                ' 保守料コメント
                Me.txtHoshuryoCmt.Text = qc001F04FormDto.TxtHoshuryoCmt
            End If
            If String.Equals(qc001F04FormDto.RdoNebikiSetteiMenuBetsuRadio, Consts.menyuu) Then
                ' メニュー別ラジオ
                Me.rdoNebikiSetteiMenuBetsuRadio.Checked = True
            Else
                ' 自動按分ラジオ
                Me.rdoNebikiSetteiZidoAnbunRadio.Checked = True
            End If
            ' 丸め設定コンボ
            Me.cmbNebikiSetteiMarumeSetteiCombo.DataSource = qc001F04FormDto.CmbNebikiSetteiMarumeSetteiCombo
            Me.cmbNebikiSetteiMarumeSetteiCombo.ValueMember = "Code"
            Me.cmbNebikiSetteiMarumeSetteiCombo.DisplayMember = "Name"
            If Not IsNothing(qc001F04FormDto.CmbNebikiSetteiMarumeSettei) Then
                Me.cmbNebikiSetteiMarumeSetteiCombo.SelectedValue = qc001F04FormDto.CmbNebikiSetteiMarumeSettei
            End If
            Me.changedFlg = True

            '#3450 Start
            ComboBoxWidth(cmbNebikiSetteiMarumeSetteiCombo)
            '#3450 End
            ClientLogUtil.Logger.DebugAP("QC001F04Form:EditGamenKomoku end")
        End Sub

        ''' <summary>
        ''' 設置先コンボ編集
        ''' </summary>
        Private Sub SetteiSettisakiCombo()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:SetteiSettisakiCombo start")
            Me.changedFlg = False
            ' 設置先コンボ
            If Not IsNothing(qc001F04FormDto.CmbMeisaiHyojiSetteiSettisakiCombo) Then
                Me.cmbMeisaiHyojiSetteiSettisakiCombo.DataSource = Copy(qc001F04FormDto.CmbMeisaiHyojiSetteiSettisakiCombo)
                Me.cmbMeisaiHyojiSetteiSettisakiCombo.ValueMember = "Code"
                Me.cmbMeisaiHyojiSetteiSettisakiCombo.DisplayMember = "Name"
                Dim senzaiFlag As Boolean = False
                For Each SetteiSettisakiCombo As ComboxOptionDto In qc001F04FormDto.CmbMeisaiHyojiSetteiSettisakiCombo
                    If String.Equals(qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki, SetteiSettisakiCombo.Code) Then
                        senzaiFlag = True
                        Exit For
                    End If
                Next
                If Not senzaiFlag Then
                    Me.cmbMeisaiHyojiSetteiSettisakiCombo.SelectedValue = Consts.zente
                Else
                    Me.cmbMeisaiHyojiSetteiSettisakiCombo.SelectedValue = qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki
                End If
            End If
            Me.changedFlg = True

            '#3450 Start
            ComboBoxWidth(cmbMeisaiHyojiSetteiSettisakiCombo)
            '#3450 End
            ClientLogUtil.Logger.DebugAP("QC001F04Form:SetteiSettisakiCombo end")
        End Sub

        ''' <summary>
        ''' 画面復元
        ''' </summary>
        Private Sub returnheda()

            Dim f04formdto As QC001F04FormDto = SharedComClient.InstanceData.QC001F04FormDTO

            If Not IsNothing(f04formdto) Then
                If Not IsNothing(f04formdto.SprHoshuRyokinSansyutsuKijunDate) Then

                    ' 保守料金算出基準日
                    qc001F04FormDto.SprHoshuRyokinSansyutsuKijunDate = f04formdto.SprHoshuRyokinSansyutsuKijunDate
                End If
                ' #8196 No5 START
                txtIchiPageNoKensuu.Text = f04formdto.PerPageSize
                qc001F04FormDto.PerPageSize = f04formdto.PerPageSize
                ' #8196 No5 END
            End If
        End Sub

        ''' <summary>
        ''' 画面項目制御
        ''' </summary>
        Private Sub ControlGamenKomoku(ByVal currentList As List(Of QC001F04M1Dto), Optional ByVal resetBackColor As Boolean = True)
            ClientLogUtil.Logger.DebugAP("QC001F04Form:ControlGamenKomoku start")
            Me.changedFlg = False

            '2022.10.07 #13563 ADD-START　付帯入力ボタン制御条件の修正
            '#12237 20220901 付帯入力ボタン非活性・活性るため追加
            'Dim hissuflag As Boolean = False
            Dim hissuflag As Boolean = True
            '2022.10.07 #13563 MOD-END　付帯入力ボタン制御条件の修正
            ' 行番号の再設定
            For Each m1Row In qc001F04FormDto.SprM1MenuIchiran
                m1Row.M1No = (qc001F04FormDto.SprM1MenuIchiran.IndexOf(m1Row) + 1).ToString
                '#12237 20220901 ADD START　付帯入力ボタン非活性・活性るため追加
                If String.Equals(m1Row.M1AddonHissu, Consts.kana) Then
                    '2022.10.07 #13563 ADD-START　付帯入力ボタン制御条件の修正
                    'If String.Equals(m1Row.CmbM1SettisakiComboCode, qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki) OrElse String.Equals("全て", qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki) Then
                    '    hissuflag = True
                    'End If
                    hissuflag = False
                    '2022.10.07 #13563 ADD-END　付帯入力ボタン制御条件の修正
                End If
                '#12237 20220901 ADD END　付帯入力ボタン非活性・活性るため追加
            Next
            '2022.10.07 #13563 ADD-START　付帯入力ボタン制御条件の修正
            If Not hissuflag Then
                Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                Dim actionMethodName As String = "CheckKeiyakuFutaiActive"
                UpdateProcessingFlagToFalse()
                hissuflag = CBool(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing))
            End If
            '2022.10.07 #13563 ADD-END　付帯入力ボタン制御条件の修正

            ' 最新単価取込
            Me.RightClickSaishinTankaTorikomi.Enabled = False
            '#IT1-0085 変更 Start
            ' 複写
            Me.RightClickCopy.Enabled = True
            '#IT1-0085 変更 End
            ' 切取
            Me.RightClickCut.Enabled = False
            ' 貼付
            Me.RightClickCopyPaste.Enabled = False
            ' 行挿入
            Me.RightClickInsert.Enabled = False
            ' 行削除
            Me.RightClickDelete.Enabled = False

            Dim NumberCellType1 As FarPoint.Win.Spread.CellType.NumberCellType = New FarPoint.Win.Spread.CellType.NumberCellType()
            NumberCellType1.DecimalPlaces = 0
            NumberCellType1.FixedPoint = False
            ' ST#6925 START
            'NumberCellType1.MaximumValue = 9999999.0R
            NumberCellType1.MaximumValue = 999999999.0R
            ' ST#6925 END
            NumberCellType1.MinimumValue = 0R
            NumberCellType1.Separator = ","
            NumberCellType1.ShowSeparator = True

            Dim NumberCellType2 As FarPoint.Win.Spread.CellType.NumberCellType = New FarPoint.Win.Spread.CellType.NumberCellType()
            NumberCellType2.DecimalPlaces = 2
            NumberCellType2.FixedPoint = True
            NumberCellType2.MaximumValue = 999.0R
            NumberCellType2.MinimumValue = 0R
            NumberCellType2.Separator = ""
            NumberCellType2.ShowSeparator = True

            If Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) AndAlso
               currentList.Count > 0 Then
                '#IT1-0085 削除 Start
                ' 複写
                'Me.RightClickCopy.Enabled = True
                '#IT1-0085 削除 End
                ' 切取
                Me.RightClickCut.Enabled = True
                ' 貼付
                Me.RightClickCopyPaste.Enabled = True
                For Each m1Dto As QC001F04M1Dto In currentList
                    If Not String.IsNullOrEmpty(m1Dto.M1MenuNo) Then
                        ' 最新単価取込
                        Me.RightClickSaishinTankaTorikomi.Enabled = True
                        Exit For
                    End If
                Next
            End If

            Dim chk As CellType.CheckBoxCellType = New CellType.CheckBoxCellType
            Dim strLst As New List(Of String)
            Dim combox As CellType.ComboBoxCellType = New CellType.ComboBoxCellType
            strLst.Add("lst1")
            strLst.Add("lst2")
            strLst.Add("lst3")
            combox.Items = strLst.ToArray
            Dim btnSuisho As CellType.ButtonCellType = New CellType.ButtonCellType
            btnSuisho.Text = Consts.kana
            Dim btnHissu As CellType.ButtonCellType = New CellType.ButtonCellType
            btnHissu.Text = Consts.osusune
            Dim txt As CellType.TextCellType = New CellType.TextCellType

            If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Change) Then
                ' 6950 strart
                '6106 Start
                If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                    'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                    '6106 End
                    ' 行挿入
                    Me.RightClickInsert.Enabled = False
                    ' 行削除
                    Me.RightClickDelete.Enabled = False
                    ' No2 画面＿明細表示設定＿設置先コンボ
                    Me.cmbMeisaiHyojiSetteiSettisakiCombo.Enabled = False
                    ' No4 画面＿保守区分コンボ
                    Me.cmbHoshuKbnCombo.Enabled = False
                    ' No6 画面＿保守料金算出基準日
                    Me.sprHoshuRyokinSansyutsuKijunDate.Enabled = False
                    ' ST#3441横展開 START
                    Me.sprHoshuRyokinSansyutsuKijunDate.ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' No12 画面＿値引設定_メニュー別ラジオ
                    Me.rdoNebikiSetteiMenuBetsuRadio.Enabled = False
                    ' No13 画面＿値引設定_自動按分ラジオ
                    Me.rdoNebikiSetteiZidoAnbunRadio.Enabled = False
                    ' No15 画面＿値引設定_丸め設定コンボ
                    Me.cmbNebikiSetteiMarumeSetteiCombo.Enabled = False
                    ' No16 画面＿メニュー選択ボタン
                    Me.btnMenuSentaku.Enabled = False
                    'ST#8852 START
                    Me.RightClickMenuSentaku.Enabled = False
                    'ST#8852 END
                    ' No17 画面＿推奨構成ボタン
                    Me.btnSuishoKosei.Enabled = False
                    ' No19 画面＿数・機器変更ボタン
                    Me.btnMenuFutai.Enabled = False
                    ' No20 画面＿料金再計算ボタン
                    Me.btnRyokinSaiKeisan.Enabled = False
                    ' No18 画面＿多拠点ボタン
                    Me.btnTaKyoten.Enabled = False
                    ' No22 画面＿正誤チェックボタン
                    Me.btnSeigoCheck.Enabled = False
                    ' No116 画面＿明細総合計ボタン
                    Me.btnMeisaiSoGokei.Enabled = False
                    ' No148 画面＿グループ変更ボタン
                    Me.btnGroupHenko.Enabled = False
                    ' No149 画面＿明細参照ボタン
                    Me.btnMeisaiSansyo.Visible = False
                    ' No149 画面＿明細参照ボタン
                    Me.btnMeisaiSansyo.Enabled = False
                Else
                    ' 行挿入
                    Me.RightClickInsert.Enabled = True
                    ' 行削除
                    Me.RightClickDelete.Enabled = True
                    ' No2 画面＿明細表示設定＿設置先コンボ
                    Me.cmbMeisaiHyojiSetteiSettisakiCombo.Enabled = True
                    ' No4 画面＿保守区分コンボ
                    Me.cmbHoshuKbnCombo.Enabled = True
                    ' No6 画面＿保守料金算出基準日
                    Me.sprHoshuRyokinSansyutsuKijunDate.Enabled = True
                    ' ST#3441横展開 START
                    Me.sprHoshuRyokinSansyutsuKijunDate.ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' No12 画面＿値引設定_メニュー別ラジオ
                    Me.rdoNebikiSetteiMenuBetsuRadio.Enabled = True
                    ' No13 画面＿値引設定_自動按分ラジオ
                    Me.rdoNebikiSetteiZidoAnbunRadio.Enabled = True
                    ' No15 画面＿値引設定_丸め設定コンボ
                    If String.Equals(qc001F04FormDto.RdoNebikiSetteiMenuBetsuRadio, Consts.menyuu) Then
                        Me.cmbNebikiSetteiMarumeSetteiCombo.Enabled = False
                    Else
                        Me.cmbNebikiSetteiMarumeSetteiCombo.Enabled = True
                    End If
                    ' No16 画面＿メニュー選択ボタン
                    Me.btnMenuSentaku.Enabled = True
                    'ST#8852 START
                    Me.RightClickMenuSentaku.Enabled = True
                    'ST#8852 END
                    ' No17 画面＿推奨構成ボタン
                    Me.btnSuishoKosei.Enabled = True
                    If qc001F04FormDto.SprM1MenuIchiran.Count > 1 Then
                        ' No18 画面＿多拠点ボタン
                        Me.btnTaKyoten.Enabled = True
                        ' No22 画面＿正誤チェックボタン
                        Me.btnSeigoCheck.Enabled = True
                        ' No116 画面＿明細総合計ボタン
                        Me.btnMeisaiSoGokei.Enabled = True
                        ' #8005
                        ' No19 画面＿数・機器変更ボタン
                        Me.btnMenuFutai.Enabled = True
                        ' No20 画面＿料金再計算ボタン
                        Me.btnRyokinSaiKeisan.Enabled = True
                    Else
                        ' No18 画面＿多拠点ボタン
                        Me.btnTaKyoten.Enabled = False
                        ' No22 画面＿正誤チェックボタン
                        Me.btnSeigoCheck.Enabled = False
                        ' No116 画面＿明細総合計ボタン
                        Me.btnMeisaiSoGokei.Enabled = False
                        ' #8005
                        ' No19 画面＿数・機器変更ボタン
                        Me.btnMenuFutai.Enabled = False
                        ' No20 画面＿料金再計算ボタン
                        Me.btnRyokinSaiKeisan.Enabled = False
                    End If
                    ' No148 画面＿グループ変更ボタン
                    Me.btnGroupHenko.Enabled = True
                    ' No149 画面＿明細参照ボタン
                    Me.btnMeisaiSansyo.Visible = True
                    ' No149 画面＿明細参照ボタン
                    Me.btnMeisaiSansyo.Enabled = True
                End If
                ' No1 明細表示設定＿設置先ラベル
                Me.lblMeisaiHyojiSetteiSettisaki.Visible = True
                ' No3 保守区分ラベル
                Me.lblHoshuKbn.Visible = True
                ' No5 保守料金算出基準日ラベル
                Me.lblHoshuRyokinSansyutsuKijunDate.Visible = True
                ' No6 画面＿保守料金算出基準日
                Me.sprHoshuRyokinSansyutsuKijunDate.Visible = True
                Me.sprHoshuRyokinSansyutsuKijunDate.TabStop = True
                ' ST#3441横展開 START
                Me.sprHoshuRyokinSansyutsuKijunDate.ImeMode = ImeMode.Disable
                ' ST#3441横展開 END
                ' No7 契約識別２ラベル
                Me.lblKeiyakuShikibetsu2.Visible = True
                ' No8 画面＿契約識別２
                Me.txtKeiyakuShikibetsu2.Visible = True
                ' No9 保守料コメントラベル
                Me.lblHoshuryoCmt.Visible = True
                ' No10 画面＿保守料コメント
                Me.txtHoshuryoCmt.Visible = True
                ' No11 値引設定ラベル
                Me.lblNebikiSettei.Visible = True
                ' No11 値引設定ラベル２
                Me.lblNebikiSettei2.Visible = True
                ' No14 丸め設定ラベル
                Me.lblMarumeSettei.Visible = True
                ' #8005
                If qc001F04FormDto.SprM1MenuIchiran.Count > 1 Then
                    '#11939 START
                    ' No21 画面＿契約付帯入力ボタン
                    Me.btnFutaiNyuryoku.Enabled = True
                    '#11939 END
                Else
                    ' No21 画面＿契約付帯入力ボタン
                    Me.btnFutaiNyuryoku.Enabled = False
                End If

                ' No23 画面＿並び替え▲ボタン
                Me.btnNarabikaeUp.Enabled = True
                ' No24 画面＿並び替えラベル
                Me.lblNarabikae.Visible = True
                ' No25 画面＿並び替え▼ボタン
                Me.btnNarabikaeDown.Enabled = True
                ' No111 画面＿全表示・設定幅ボタン
                Me.btnAllHyojiSetteiHaba.Enabled = True
                ' No112 画面＿幅記憶ボタン
                Me.btnHabaKioku.Enabled = True
                ' No113 合計表示設定ラベル
                Me.lblGokeiHyojiSettei.Visible = False
                ' No114 画面＿合計表示設定_保守ラジオ
                Me.rdoGokeiHyojiSetteiHoshuRadio.Visible = False
                ' No115 画面＿合計表示設定_課金ラジオ
                Me.rdoGokeiHyojiSetteiKakinRadio.Visible = False
                ' No149 画面＿明細参照ボタン
                Me.btnMeisaiSansyo.Visible = True
                ' 6950 end

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM1MenuIchiran.SuspendLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                ' 2021/08/13 #4813 これより先にＳｐｒｅａｄ設定されていればこのままでＯＫ
                'For rowCnt = 0 To Me.sprM1MenuIchiran_Sheet1.RowCount - 1
                For rowCnt = 0 To currentList.Count - 1

                    Me.sprM1MenuIchiran_Sheet1.Rows(rowCnt).Visible = True

                    'ST1_#4936 START
                    For colIdx As Integer = 0 To 31
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, colIdx).BackColor = Drawing.Color.Empty
                    Next

                    'ST#7255
                    ' 6950 start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        'ST1_#4936 END
                        sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(169, 169, 169)
                        ' 6950 end
                    ElseIf String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ka) Then
                        'ST1_#4936 END
                        If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) Then
                            If String.IsNullOrEmpty(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"), String)) Then
                                sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(204, 255, 255)
                            Else
                                sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                            End If
                        ElseIf String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU2) Then
                            sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(204, 255, 255)
                        Else
                            sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                        End If
                    Else
                        sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    End If

                    ' Ｍ１＿Ｎｏ
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1No).Locked = True
                    ' Ｍ１＿メニュー番号
                    ' 6950 start
                    '6106 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).BackColor = Drawing.Color.White
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿メニュー名称
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNm).Locked = True
                    ' Ｍ１＿種別
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Locked = True
                    ' Ｍ１＿契約単位
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1KeiyakuTani).Locked = True
                    ' Ｍ１＿請求
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Locked = True
                    ' Ｍ１＿付帯
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Futai).Locked = True
                    ' Ｍ１＿アドオン（必須）
                    If String.Equals(currentList(rowCnt).M1AddonHissu, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = True
                    Else
                        ' 障害_ST先行検証 #10662 Start
                        ' 6950 start
                        '6106 Start
                        'If ((String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ho) AndAlso
                        '   Me.rdoNebikiSetteiZidoAnbunRadio.Checked)) OrElse
                        '   String.Equals(currentList(rowCnt).Status, "3") OrElse
                        '   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        If String.Equals(currentList(rowCnt).Status, "3") OrElse
                           String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = btnSuisho
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = btnSuisho
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).BackColor = Drawing.Color.White
                        End If
                        ' 障害_ST先行検証 #10662 End
                    End If
                    ' Ｍ１＿アドオン（推奨）
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1AddonSuisho, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = True
                    Else
                        ' 障害_ST先行検証 #10662 Start
                        ' 6950 start
                        '6106 Start
                        'If ((String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ho) AndAlso
                        '   Me.rdoNebikiSetteiZidoAnbunRadio.Checked)) OrElse
                        '   String.Equals(currentList(rowCnt).Status, "3") OrElse
                        '   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        If String.Equals(currentList(rowCnt).Status, "3") OrElse
                           String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = btnHissu
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = btnHissu
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).BackColor = Drawing.Color.White
                        End If
                        ' 障害_ST先行検証 #10662 End
                    End If
                    ' Ｍ１＿委託希望
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(qc001F04FormDto.RdoNebikiSetteiMenuBetsuRadio, Consts.jidoubunsuu) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                    Else
                        If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                            If String.Equals(currentList(rowCnt).M1ItakuKibo, Consts.checKbox.checktrue) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = chk
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Value = True
                            Else
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = chk
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Value = False
                            End If
                            If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailComDto, "GROUPNO"), Consts.GROUPNO.ari) AndAlso
                           String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailHosDto, "PACK345_FLG"), Consts.PACK345FLG.PACK345iie) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = False
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                            End If
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = txt
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Text = Consts.ousenn
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                        End If
                    End If
                    ' 6950 start
                    '6106 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                    End If
                    ' Ｍ１＿数量
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Sryo).Locked = True
                    ' Ｍ１＿年額定価
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1NengakuTeika, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).CellType = NumberCellType1
                    End If
                    If Me.rdoNebikiSetteiMenuBetsuRadio.Checked AndAlso
                       String.Equals(currentList(rowCnt).M1ItakuKibo, Consts.checKbox.checktrue) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Locked = False
                        If resetBackColor Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).BackColor = Drawing.Color.White
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Locked = True
                    End If
                    ' 6950 start
                    '6106 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿月額定価
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1GetsugakuTeika, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).Text = Consts.ousenn
                    Else
                        'ST1#7219 START
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).CellType = NumberCellType1
                        'ST1#7219 END
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).Locked = True
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿年額売価単価
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1NengakuBinTnk, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).CellType = NumberCellType1
                    End If
                    ' #8260 T)Annaka Start
                    'If (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                    '   Me.rdoNebikiSetteiMenuBetsuRadio.Checked) OrElse
                    '   (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                    '   String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Value, Consts.nen)) Then
                    If (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                        Me.rdoNebikiSetteiMenuBetsuRadio.Checked) OrElse
                       (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                        String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Value, Consts.nen) AndAlso
                        String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailComDto, "AK_MENUNEBIKI_FLG"), Consts.ari)) Then
                        ' #8260 T)Annaka End
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Locked = False
                        If resetBackColor Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).BackColor = Drawing.Color.White
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Locked = True
                    End If
                    ' 6950 start
                    '6106 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿月額売価単価
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1GetsugakuBinTnk, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).CellType = NumberCellType1
                    End If
                    ' #8260 T)Annaka Start
                    'If (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                    '   Me.rdoNebikiSetteiMenuBetsuRadio.Checked) OrElse
                    '   (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                    '   String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Value, Consts.tsuki)) Then
                    If (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                        Me.rdoNebikiSetteiMenuBetsuRadio.Checked) OrElse
                       (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                        String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Value, Consts.tsuki) AndAlso
                        String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailComDto, "AK_MENUNEBIKI_FLG"), Consts.ari)) Then
                        ' #8260 T)Annaka End
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Locked = False
                        If resetBackColor Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).BackColor = Drawing.Color.White
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Locked = True
                    End If
                    ' 6950 start
                    '6106 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿年額値引％
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1NengakuNebikiPar, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).CellType = NumberCellType2
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).Locked = True
                    ' Ｍ１＿月額値引％
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1GetsugakuNebikiPar, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).CellType = NumberCellType2
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).Locked = True
                    ' Ｍ１＿年額費用
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1NengakuHiyo, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).Locked = True
                    ' Ｍ１＿月額費用
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(currentList(rowCnt).M1GetsugakuHiyo, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).Locked = True
                    ' Ｍ１＿無償（初期費用）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        If String.Equals(currentList(rowCnt).M1MusyoShokiHiyo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = False
                        End If
                        '#11373 チェックボックスの値を確定させるため、Focusを再設定する
                        Me.sprM1MenuIchiran.Focus()
                        'ST1_2811 START
                        If CDec(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "SYOKI_HIYO")) = 0 Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                        Else
                            If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN2) OrElse
                                String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN3) OrElse
                                (String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN4) AndAlso
                                qc001F04FormDto.syoriRes = 0) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = False
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                            End If
                        End If
                        'ST1_2811 END
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                    End If
                    ' 6950 start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿初期費用
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ShokiHiyo).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ShokiHiyo).Locked = True
                    ' Ｍ１＿無償（随時費用）

                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        If String.Equals(currentList(rowCnt).M1MusyoZuijiHiyo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = False
                        End If
                        '#11373 横展開 チェックボックスの値を確定させるため、Focusを再設定する
                        Me.sprM1MenuIchiran.Focus()
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).BackColor = Drawing.Color.White
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = True
                    End If
                    ' 6950 start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = True

                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿随時費用
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ZuijiHiyo).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ZuijiHiyo).Locked = True
                    ' Ｍ１＿原価区分
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        combox = New CellType.ComboBoxCellType
                        strLst = New List(Of String)
                        If currentList(rowCnt).M1GnkKbn IsNot Nothing AndAlso currentList(rowCnt).M1GnkKbn.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GnkKbn
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                        combox.Items = strLst.ToArray
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).CellType = combox
                        For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GnkKbn
                            If String.Equals(currentList(rowCnt).M1GnkKbnChoose, ComboxOptionDto.Code) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Value = ComboxOptionDto.Name
                            End If
                        Next
                        If Me.rdoNebikiSetteiMenuBetsuRadio.Checked Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).BackColor = Drawing.Color.White
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                    End If
                    ' 6950 start
                    '6106 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                    End If
                    ' Ｍ１＿標準原価
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).CellType = NumberCellType1
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                       String.Equals(currentList(rowCnt).KOBETUFLG, Consts.KOBETUFLG) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).BackColor = Drawing.Color.White
                    ElseIf String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                       String.Equals(currentList(rowCnt).GENCALCKBN, Consts.GENCALCKBN.GENCALCKBN_9) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = False
                        If resetBackColor Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).BackColor = Drawing.Color.White
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = True
                    End If
                    ' 6950 start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿後粗利
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1AtoArari).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1AtoArari).Locked = True
                    ' Ｍ１＿粗利％
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ArariPar).CellType = NumberCellType2
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ArariPar).Locked = True
                    ' Ｍ１＿月額無償（月数）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then

                        combox = New CellType.ComboBoxCellType
                        strLst = New List(Of String)
                        If currentList(rowCnt).M1GetsugakuMusyoMoNum IsNot Nothing AndAlso currentList(rowCnt).M1GetsugakuMusyoMoNum.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GetsugakuMusyoMoNum
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                        combox.Items = strLst.ToArray
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).CellType = combox
                        If Not String.IsNullOrEmpty(currentList(rowCnt).M1GetsugakuMusyoMoNumChoose) Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GetsugakuMusyoMoNum
                                If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).M1GetsugakuMusyoMoNumChoose) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Value = ComboxOptionDto.Name
                                End If
                            Next
                        End If
                        If String.Equals(strLst.Count, 0) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                        ElseIf strLst.Count = 1 Then
                            If (String.Equals(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), String), Consts.MUSYODISPKBN.MUSYODISPKBN2) OrElse
                                String.Equals(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), String), Consts.MUSYODISPKBN.MUSYODISPKBN3) OrElse
                                String.Equals(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), String), Consts.MUSYODISPKBN.MUSYODISPKBN4)) AndAlso
                                String.Equals(strLst(0), "0") Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                            End If
                        Else
                            'ST1_2811 START
                            If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN2) OrElse
                                    String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN3) OrElse
                                    (String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN4) AndAlso
                                    qc001F04FormDto.syoriRes = 0) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = False
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).BackColor = Drawing.Color.White
                            Else
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                            End If
                            'ST1_2811 END
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                    End If
                    ' 6950 start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                    End If
                    ' Ｍ１＿設置先コンボ
                    combox = New CellType.ComboBoxCellType
                    strLst = New List(Of String)
                    If qc001F04FormDto.CmbM1SettisakiCombo IsNot Nothing AndAlso qc001F04FormDto.CmbM1SettisakiCombo.Count > 0 Then
                        For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.CmbM1SettisakiCombo
                            strLst.Add(ComboxOptionDto.Name)
                        Next
                    End If
                    combox.Items = strLst.ToArray
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).CellType = combox
                    If Not String.IsNullOrEmpty(currentList(rowCnt).CmbM1SettisakiComboCode) Then
                        '障害_ST先行検証 #10730 Start
                        'ST1_#7945 START
                        For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.CmbM1SettisakiCombo
                            If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).CmbM1SettisakiComboCode) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Text = ComboxOptionDto.Name
                            End If
                        Next
                        'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Text = currentList(rowCnt).CmbM1SettisakiComboChoose
                        'ST1_#7945 END
                        '障害_ST先行検証 #10730 End
                    End If

                    ' ST#6907 START
                    ' 障害_ST先行検証 #10662 IT #213 Start
                    'If String.IsNullOrWhiteSpace(currentList(rowCnt).M1MenuNo) OrElse
                    '    String.Equals(currentList(rowCnt).Status, "3") OrElse
                    '    String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then

                    '#IT1-1015 変更 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                        String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") OrElse
                        String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        '#IT1-1015 変更 End
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = True
                    Else
                        '障害_ST先行検証 #10730 Start
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                        'If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        '    If Me.rdoNebikiSetteiZidoAnbunRadio.Checked Then
                        '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = True
                        '    Else
                        '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                        '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                        '    End If
                        'ElseIf String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ka) Then
                        '    If Not String.IsNullOrWhiteSpace(currentList(rowCnt).SerMenuno) Then
                        '        If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) OrElse
                        '            String.Equals(currentList(rowCnt).SerMenuno, currentList(rowCnt).M1NaibuNo) Then

                        '            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                        '            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                        '        Else
                        '            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = True
                        '        End If
                        '    Else
                        '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                        '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                        '    End If
                        'End If
                        '障害_ST先行検証 #10730 End
                    End If
                    ' 障害_ST先行検証 #10662 IT #213 End
                    ' ST#6907 END

                    ' Ｍ１＿グループコンボ
                    combox = New CellType.ComboBoxCellType
                    strLst = New List(Of String)
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        If qc001F04FormDto.HoxyuCmbM1GroupCombo IsNot Nothing Then
                            For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.HoxyuCmbM1GroupCombo
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                    Else
                        If qc001F04FormDto.KakinCmbM1GroupCombo IsNot Nothing AndAlso qc001F04FormDto.KakinCmbM1GroupCombo.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.KakinCmbM1GroupCombo
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                    End If
                    combox.Items = strLst.ToArray
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).CellType = combox
                    If Not String.IsNullOrEmpty(currentList(rowCnt).CmbM1GroupComboCode) Then
                        If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                            For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.HoxyuCmbM1GroupCombo
                                If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).CmbM1GroupComboCode) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Value = ComboxOptionDto.Name
                                End If
                            Next
                        Else
                            For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.KakinCmbM1GroupCombo
                                If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).CmbM1GroupComboCode) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Value = ComboxOptionDto.Name
                                End If
                            Next
                        End If
                    End If
                    ' 障害_ST先行検証 #10662 IT #213 Start
                    'If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                    '   Me.rdoNebikiSetteiZidoAnbunRadio.Checked AndAlso
                    '   String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Locked = True
                    'Else
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Locked = False
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).BackColor = Drawing.Color.White
                    'End If
                    ' 障害_ST先行検証 #10662 IT #213 End
                    ' 6950 start
                    '6106 Start
                    '#IT1-1015 変更 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        '#IT1-1015 変更 End
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Locked = True
                    End If
                    ' Ｍ１＿サブタイトルチェック
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).CellType = chk
                    If currentList(rowCnt).M1SubTtl Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Value = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Value = False
                    End If
                    If Not String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) OrElse
                       String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailComDto, "SUBTITLEKBN"), Consts.SUBTITLEKBN.SUBTITLEKBN1) AndAlso
                            Me.rdoNebikiSetteiMenuBetsuRadio.Checked Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).BackColor = Drawing.Color.White
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = True
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = True
                    End If
                    ' 6950 start
                    '6106 Start
                    If String.Equals(currentList(rowCnt).Status, "3") OrElse
                       String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                        'String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "3") Then
                        '6106 End
                        ' 6950 end
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = True
                    End If
                    ' Ｍ１＿サブタイトル
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1SubTtl).Locked = True
                Next
                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM1MenuIchiran.ResumeLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM2GokeiIchiran.SuspendLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                For rowCnt = 0 To Me.sprM2GokeiIchiran_Sheet1.RowCount - 1
                    If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).M2hyoujiFlag, Consts.hihyouji) Then
                        Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = False
                    Else
                        If String.IsNullOrEmpty(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2GokeiShbt2) Then
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            Me.sprM2GokeiIchiran_Sheet1.Cells(rowCnt, 0).Border = New FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Lowered, Drawing.Color.Black, Drawing.Color.LightGray, 1)
                            Me.sprM2GokeiIchiran_Sheet1.AddSpanCell(rowCnt, 0, 1, 2)
                        Else
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                        End If
                        If Not String.IsNullOrEmpty(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2GokeiShbt2) Then
                            Me.sprM2GokeiIchiran_Sheet1.Cells(rowCnt, 0).Border = New FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Lowered, Drawing.Color.Black, Drawing.Color.LightGray, 1)
                        End If
                        If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisakiNull) OrElse
                           String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.maehoshiSettisakiNull) Then
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            '契約変更たよ明細タブ合計欄表示不正対応 START
                            'Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).CellType = txt
                            '契約変更たよ明細タブ合計欄表示不正対応 END
                        End If
                        ' Ｍ２＿合計種別
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.sprM2GokeiShbt).Locked = True
                        ' Ｍ２＿合計種別2
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.sprM2GokeiShbt2).Locked = True
                        ' Ｍ２＿合計欄_年額定価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuTeika).Locked = True
                        ' Ｍ２＿合計欄_月額定価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuTeika).Locked = True
                        ' Ｍ２＿合計欄_初期費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranShokiHiyo).Locked = True
                        ' Ｍ２＿合計欄_随時費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranZuijiHiyo).Locked = True
                        ' Ｍ２＿合計欄_年額値引額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuNebikigaku).Locked = True
                        ' Ｍ２＿合計欄_月額値引額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuNebikigaku).Locked = True
                        ' Ｍ２＿合計欄_年額費用
                        'ST1_#6106 START
                        'If Me.rdoNebikiSetteiZidoAnbunRadio.Checked Then
                        If Me.rdoNebikiSetteiZidoAnbunRadio.Checked AndAlso Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                            'ST1_#6106 END
                            If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiGokei) OrElse
                               String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki) Then
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = False
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = True
                            End If
                        Else
                            Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = True
                        End If
                        ' ST#3441横展開 START
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).ImeMode = ImeMode.Disable
                        ' ST#3441横展開 END
                        ' Ｍ２＿合計欄_月額費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuHiyo).Locked = True
                        ' Ｍ２＿合計欄_標準原価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranHyojunGnk).Locked = True
                        ' Ｍ２＿合計欄_粗利額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranArarigaku).Locked = True
                        ' Ｍ２＿合計欄_粗利％
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranArariPar).Locked = True
                        ' Ｍ２＿月額換算後欄_月額費用
                        'ST1_#6106 START
                        'If Me.rdoNebikiSetteiZidoAnbunRadio.Checked Then
                        If Me.rdoNebikiSetteiZidoAnbunRadio.Checked AndAlso Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                            'ST1_#6106 END
                            If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiGokei) OrElse
                               String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki) Then
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = False
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = True
                            End If
                        Else
                            Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = True
                        End If
                        '6106 Start
                        Dim setNo As String
                        Dim seigyoFlg As Boolean = False
                        If Not Me.cmbMeisaiHyojiSetteiSettisakiCombo.Text.Equals(Consts.zente) Then
                            For Each tempMeisai In qc001F04FormDto.SprM1MenuIchiran
                                ' 設置先がComboBoxの選択値と一致またはNothing（最後の空行）
                                If Me.cmbMeisaiHyojiSetteiSettisakiCombo.Text.Equals(tempMeisai.CmbM1SettisakiComboChoose) Then
                                    ' 共有_たよトラン情報
                                    Dim qc001Mjta = If(SharedComClient.InstanceData.QC001_MJTA, New QC001_MJTA)
                                    Dim qc001MjtakeiDto = qc001Mjta.QC001_MJTAKEIDTOList.Find(Function(o) String.Equals(o.MJTAKEIDTO.KEY_MJ, tempMeisai.HENKOMAE_KEY_MJ) AndAlso
                                                                                                  String.Equals(o.MJTAKEIDTO.KEY_MJ_HAN, tempMeisai.HENKOMAE_KEY_MJ_HAN) AndAlso
                                                                                                  String.Equals(o.MJTAKEIDTO.KEY_GEN, tempMeisai.HENKOMAE_KEY_GEN) AndAlso
                                                                                                  String.Equals(o.MJTAKEIDTO.KEY_EDA, tempMeisai.HENKOMAE_KEY_EDA))
                                    If Not String.Equals(qc001MjtakeiDto.MJTAKEIDTO.STATUS, "3") Then
                                        seigyoFlg = True
                                        Exit For
                                    End If
                                End If
                            Next
                            If Not seigyoFlg OrElse String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori, "1") Then
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = True
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = True
                            End If
                        End If
                        '6106 End
                        ' ST#3441横展開 START
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).ImeMode = ImeMode.Disable
                        ' ST#3441横展開 END
                        ' Ｍ２＿月額換算後欄_標準原価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranHyojunGnk).Locked = True
                        ' Ｍ２＿月額換算後欄_粗利額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranArarigaku).Locked = True
                        ' Ｍ２＿月額換算後欄_粗利％
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranArariPar).Locked = True
                    End If
                Next

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM2GokeiIchiran.ResumeLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

            ElseIf String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Modify) OrElse
                   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.DemoKirikae) OrElse
                   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.DemoKasidasi) OrElse
                   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Sinsei) OrElse
                   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Print) OrElse
                  String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.TanNendoUpdate) Then
                ' No1 明細表示設定＿設置先ラベル
                Me.lblMeisaiHyojiSetteiSettisaki.Visible = True
                ' No2 画面＿明細表示設定＿設置先コンボ
                Me.cmbMeisaiHyojiSetteiSettisakiCombo.Enabled = True
                ' No3 保守区分ラベル
                Me.lblHoshuKbn.Visible = True
                ' No4 画面＿保守区分コンボ
                Me.cmbHoshuKbnCombo.Enabled = False
                ' No5 保守料金算出基準日ラベル
                Me.lblHoshuRyokinSansyutsuKijunDate.Visible = True
                ' No6 画面＿保守料金算出基準日
                Me.sprHoshuRyokinSansyutsuKijunDate.Visible = True
                ' No6 画面＿保守料金算出基準日
                Me.sprHoshuRyokinSansyutsuKijunDate.Enabled = False
                Me.sprHoshuRyokinSansyutsuKijunDate.TabStop = False
                ' ST#3441横展開 START
                Me.sprHoshuRyokinSansyutsuKijunDate.ImeMode = ImeMode.Disable
                ' ST#3441横展開 END
                ' No7 契約識別２ラベル
                Me.lblKeiyakuShikibetsu2.Visible = False
                ' No8 画面＿契約識別２
                Me.txtKeiyakuShikibetsu2.Visible = False
                ' No9 保守料コメントラベル
                Me.lblHoshuryoCmt.Visible = False
                ' No10 画面＿保守料コメント
                Me.txtHoshuryoCmt.Visible = False
                ' No11 値引設定ラベル
                Me.lblNebikiSettei.Visible = True
                ' No11 値引設定ラベル２
                Me.lblNebikiSettei2.Visible = True
                ' No12 画面＿値引設定_メニュー別ラジオ
                Me.rdoNebikiSetteiMenuBetsuRadio.Enabled = False
                ' No13 画面＿値引設定_自動按分ラジオ
                Me.rdoNebikiSetteiZidoAnbunRadio.Enabled = False
                ' No14 丸め設定ラベル
                Me.lblMarumeSettei.Enabled = False
                ' No15 画面＿値引設定_丸め設定コンボ
                Me.cmbNebikiSetteiMarumeSetteiCombo.Enabled = False
                ' No16 画面＿メニュー選択ボタン
                Me.btnMenuSentaku.Enabled = False
                'ST#8852 START
                Me.RightClickMenuSentaku.Enabled = False
                'ST#8852 END
                ' No17 画面＿推奨構成ボタン
                Me.btnSuishoKosei.Enabled = False
                ' No19 画面＿数・機器変更ボタン
                Me.btnMenuFutai.Enabled = False
                ' No20 画面＿料金再計算ボタン
                Me.btnRyokinSaiKeisan.Enabled = False
                ' No22 画面＿正誤チェックボタン
                Me.btnSeigoCheck.Enabled = False
                ' No23 画面＿並び替え▲ボタン
                Me.btnNarabikaeUp.Enabled = True
                ' No24 画面＿並び替えラベル
                Me.lblNarabikae.Visible = True
                ' No25 画面＿並び替え▼ボタン
                Me.btnNarabikaeDown.Enabled = True
                ' No111 画面＿全表示・設定幅ボタン
                Me.btnAllHyojiSetteiHaba.Enabled = True
                ' No112 画面＿幅記憶ボタン
                Me.btnHabaKioku.Enabled = True
                ' No113 合計表示設定ラベル
                Me.lblGokeiHyojiSettei.Visible = True
                ' No114 画面＿合計表示設定_保守ラジオ
                Me.rdoGokeiHyojiSetteiHoshuRadio.Enabled = True
                ' No115 画面＿合計表示設定_課金ラジオ
                Me.rdoGokeiHyojiSetteiKakinRadio.Enabled = True
                ' ST1_5030 START
                ' No18 画面＿多拠点ボタン
                Me.btnTaKyoten.Enabled = False
                ' ST1_5030 END
                If qc001F04FormDto.SprM1MenuIchiran.Count > 0 Then
                    ' ST1_5030 START
                    ' No18 画面＿多拠点ボタン
                    'Me.btnTaKyoten.Enabled = True
                    ' ST1_5030 START
                    ' No116 画面＿明細総合計ボタン
                    Me.btnMeisaiSoGokei.Enabled = True
                    ' #8005
                    ' No21 画面＿契約付帯入力ボタン
                    '2022.10.07 #13563 MOD-START　付帯入力ボタン制御条件の修正
                    '#12237 20220908 MOD-START　付帯入力ボタン非活性・活性るため追加
                    'If hissuflag Then
                    '    If qc001F04FormDto.CmbM1SettisakiCombo.Count > 1 Then
                    '        Me.btnFutaiNyuryoku.Enabled = True
                    '    Else
                    '        Me.btnFutaiNyuryoku.Enabled = False
                    '    End If
                    'ElseIf Not hissuflag Then
                    '    Me.btnFutaiNyuryoku.Enabled = True
                    'End If
                    '#12237 20220908 MOD-END　付帯入力ボタン非活性・活性るため追加
                    Me.btnFutaiNyuryoku.Enabled = hissuflag
                    '2022.10.07 #13563 MOD-END　付帯入力ボタン制御条件の修正
                Else
                    ' ST1_5030 START
                    ' No18 画面＿多拠点ボタン
                    'Me.btnTaKyoten.Enabled = False
                    ' ST1_5030 START
                    ' No116 画面＿明細総合計ボタン
                    Me.btnMeisaiSoGokei.Enabled = False
                    ' #8005
                    ' No21 画面＿契約付帯入力ボタン
                    Me.btnFutaiNyuryoku.Enabled = False
                End If
                ' No148 画面＿グループ変更ボタン
                Me.btnGroupHenko.Enabled = False
                ' No149 画面＿明細参照ボタン
                Me.btnMeisaiSansyo.Visible = False

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM1MenuIchiran.SuspendLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                ' 2021/08/13 #4813 これより先にＳｐｒｅａｄ設定されていればこのままでＯＫ
                'For rowCnt = 0 To Me.sprM1MenuIchiran_Sheet1.RowCount - 1
                For rowCnt = 0 To currentList.Count - 1

                    'ST1_#4936 START
                    For colIdx As Integer = 0 To 31
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, colIdx).BackColor = Drawing.Color.Empty
                    Next

                    'ST#7255
                    If String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ka) Then
                        'ST1_#4936 END
                        If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) Then
                            If String.IsNullOrEmpty(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"), String)) Then
                                sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(204, 255, 255)
                            Else
                                sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                            End If
                        ElseIf String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU2) Then
                            sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(204, 255, 255)
                        Else
                            sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                        End If
                    Else
                        sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    End If

                    ' Ｍ１＿Ｎｏ
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1No).Locked = True
                    ' Ｍ１＿メニュー番号
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).Locked = True
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿メニュー名称
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNm).Locked = True
                    ' Ｍ１＿種別
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Locked = True
                    ' Ｍ１＿契約単位
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1KeiyakuTani).Locked = True
                    ' Ｍ１＿請求
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Locked = True
                    ' Ｍ１＿付帯
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Futai).Locked = True
                    ' Ｍ１＿アドオン（必須）
                    If String.Equals(currentList(rowCnt).M1AddonHissu, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = True
                    Else
                        ' 障害_ST先行検証 #10662 Start
                        'If (String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ho) AndAlso
                        '        Me.rdoNebikiSetteiZidoAnbunRadio.Checked) Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = btnSuisho
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = True
                        'Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = btnSuisho
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).BackColor = Drawing.Color.White
                        'End If
                        ' 障害_ST先行検証 #10662 End
                    End If
                    ' Ｍ１＿アドオン（推奨）
                    If String.Equals(currentList(rowCnt).M1AddonSuisho, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = True
                    Else
                        ' 障害_ST先行検証 #10662 Start
                        'If (String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ho) AndAlso
                        '       Me.rdoNebikiSetteiZidoAnbunRadio.Checked) Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = btnHissu
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = True
                        'Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = btnHissu
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).BackColor = Drawing.Color.White
                        'End If
                        ' 障害_ST先行検証 #10662 End
                    End If
                    ' Ｍ１＿委託希望
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = chk
                        If String.Equals(currentList(rowCnt).M1ItakuKibo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Value = False
                        End If
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                    End If
                    ' Ｍ１＿数量
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Sryo).Locked = True
                    ' Ｍ１＿年額定価
                    If String.Equals(currentList(rowCnt).M1NengakuTeika, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Locked = True
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿月額定価
                    If String.Equals(currentList(rowCnt).M1GetsugakuTeika, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).Locked = True

                    ' Ｍ１＿年額売価単価
                    If String.Equals(currentList(rowCnt).M1NengakuBinTnk, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Locked = True
                    'ST#7255 課金の場合、編集不可の項目は色を青くする
                    'If resetBackColor Then
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    'End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿月額売価単価
                    If String.Equals(currentList(rowCnt).M1GetsugakuBinTnk, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Locked = True
                    'ST#7255 課金の場合、編集不可の項目は色を青くする
                    'If resetBackColor Then
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    'End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿年額値引％
                    If String.Equals(currentList(rowCnt).M1NengakuNebikiPar, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).CellType = NumberCellType2
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).Locked = True
                    ' Ｍ１＿月額値引％
                    If String.Equals(currentList(rowCnt).M1GetsugakuNebikiPar, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).CellType = NumberCellType2
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).Locked = True
                    ' Ｍ１＿年額費用
                    If String.Equals(currentList(rowCnt).M1NengakuHiyo, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).Locked = True
                    ' Ｍ１＿月額費用
                    If String.Equals(currentList(rowCnt).M1GetsugakuHiyo, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).Locked = True
                    ' Ｍ１＿無償（初期費用）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        If String.Equals(currentList(rowCnt).M1MusyoShokiHiyo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = False
                        End If
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                        '#11373 チェックボックスの値を確定させるため、Focusを再設定する
                        Me.sprM1MenuIchiran.Focus()
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿初期費用
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ShokiHiyo).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ShokiHiyo).Locked = True
                    ' Ｍ１＿無償（随時費用）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        If String.Equals(currentList(rowCnt).M1MusyoZuijiHiyo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = False
                        End If
                        '#11373 横展開 チェックボックスの値を確定させるため、Focusを再設定する
                        Me.sprM1MenuIchiran.Focus()
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿随時費用
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ZuijiHiyo).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ZuijiHiyo).Locked = True
                    ' Ｍ１＿原価区分
                    'ST1_#4801 START
                    'If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) OrElse
                        (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso currentList(rowCnt).M1GnkKbn.Count > 0) Then
                        'ST1_#4801 END
                        combox = New CellType.ComboBoxCellType
                        strLst = New List(Of String)
                        If currentList(rowCnt).M1GnkKbn IsNot Nothing AndAlso currentList(rowCnt).M1GnkKbn.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GnkKbn
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                        combox.Items = strLst.ToArray
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).CellType = combox
                        For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GnkKbn
                            If String.Equals(currentList(rowCnt).M1GnkKbnChoose, ComboxOptionDto.Code) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Value = ComboxOptionDto.Name
                            End If
                        Next

                        'ST1_#4801 START
                        'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                        'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                        If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                        ElseIf String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso currentList(rowCnt).M1GnkKbn.Count > 0 Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).BackColor = Drawing.Color.White
                        End If
                        'ST1_#4801 END
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                    End If
                    ' Ｍ１＿標準原価
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).CellType = NumberCellType1
                    'ST1_#4801 START
                    'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = True
                    'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                        String.Equals(currentList(rowCnt).KOBETUFLG, Consts.KOBETUFLG) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).BackColor = Drawing.Color.White
                    ElseIf String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                        String.Equals(Trim(currentList(rowCnt).M1GnkKbnChoose), "Z") Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).BackColor = Drawing.Color.White
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = True
                    End If
                    'ST1_#4801 END
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿後粗利
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1AtoArari).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1AtoArari).Locked = True
                    ' Ｍ１＿粗利％
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ArariPar).CellType = NumberCellType2
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ArariPar).Locked = True
                    ' Ｍ１＿月額無償（月数）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        combox = New CellType.ComboBoxCellType
                        strLst = New List(Of String)
                        If currentList(rowCnt).M1GetsugakuMusyoMoNum IsNot Nothing AndAlso currentList(rowCnt).M1GetsugakuMusyoMoNum.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GetsugakuMusyoMoNum
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                        combox.Items = strLst.ToArray
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).CellType = combox
                        If Not String.IsNullOrEmpty(currentList(rowCnt).M1GetsugakuMusyoMoNumChoose) Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GetsugakuMusyoMoNum
                                If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).M1GetsugakuMusyoMoNumChoose) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Value = ComboxOptionDto.Name
                                End If
                            Next
                        End If
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                    End If
                    ' Ｍ１＿設置先コンボ
                    combox = New CellType.ComboBoxCellType
                    strLst = New List(Of String)
                    If qc001F04FormDto.CmbM1SettisakiCombo IsNot Nothing AndAlso qc001F04FormDto.CmbM1SettisakiCombo.Count > 0 Then
                        For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.CmbM1SettisakiCombo
                            strLst.Add(ComboxOptionDto.Name)
                        Next
                    End If
                    combox.Items = strLst.ToArray
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).CellType = combox
                    If Not String.IsNullOrEmpty(currentList(rowCnt).CmbM1SettisakiComboCode) Then
                        For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.CmbM1SettisakiCombo
                            If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).CmbM1SettisakiComboCode) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Value = ComboxOptionDto.Name
                            End If
                        Next
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                    ' Ｍ１＿グループコンボ
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).CellType = txt
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Locked = True
                    ' Ｍ１＿サブタイトルチェック
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).CellType = chk
                    If currentList(rowCnt).M1SubTtl Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Value = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Value = False
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = True
                    ' Ｍ１＿サブタイトル
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1SubTtl).Locked = True

                Next

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM1MenuIchiran.ResumeLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM2GokeiIchiran.SuspendLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                For rowCnt = 0 To Me.sprM2GokeiIchiran_Sheet1.RowCount - 1
                    If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).M2hyoujiFlag, Consts.hihyouji) Then
                        Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = False
                    Else
                        If String.IsNullOrEmpty(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2GokeiShbt2) Then
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            Me.sprM2GokeiIchiran_Sheet1.Cells(rowCnt, 0).Border = New FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Lowered, Drawing.Color.Black, Drawing.Color.LightGray, 1)
                            Me.sprM2GokeiIchiran_Sheet1.AddSpanCell(rowCnt, 0, 1, 2)
                        ElseIf String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).TxtM2Bango, qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki) Then
                            If (Me.rdoGokeiHyojiSetteiHoshuRadio.Checked AndAlso String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki)) OrElse
                               (Me.rdoGokeiHyojiSetteiKakinRadio.Checked AndAlso String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.kakinSettisaki)) Then
                                Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            End If
                        End If
                        If Not String.IsNullOrEmpty(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2GokeiShbt2) Then
                            Me.sprM2GokeiIchiran_Sheet1.Cells(rowCnt, 0).Border = New FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Lowered, Drawing.Color.Black, Drawing.Color.LightGray, 1)
                        End If
                        If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisakiNull) OrElse
                           String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.kakinSettisakiNull) Then
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            '契約変更たよ明細タブ合計欄表示不正対応 START
                            'Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).CellType = txt
                            '契約変更たよ明細タブ合計欄表示不正対応 END
                        End If
                        ' Ｍ２＿合計種別
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.sprM2GokeiShbt).Locked = True
                        ' Ｍ２＿合計種別2
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.sprM2GokeiShbt2).Locked = True
                        ' Ｍ２＿合計欄_年額定価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuTeika).Locked = True
                        ' Ｍ２＿合計欄_月額定価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuTeika).Locked = True
                        ' Ｍ２＿合計欄_初期費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranShokiHiyo).Locked = True
                        ' Ｍ２＿合計欄_随時費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranZuijiHiyo).Locked = True
                        ' Ｍ２＿合計欄_年額値引額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuNebikigaku).Locked = True
                        ' Ｍ２＿合計欄_月額値引額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuNebikigaku).Locked = True
                        ' Ｍ２＿合計欄_年額費用
                        If Me.rdoNebikiSetteiZidoAnbunRadio.Checked Then
                            If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiGokei) OrElse
                               String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki) Then
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = False
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = True
                            End If
                        Else
                            Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = True
                        End If
                        ' ST#3441横展開 START
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).ImeMode = ImeMode.Disable
                        ' ST#3441横展開 END
                        ' Ｍ２＿合計欄_月額費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuHiyo).Locked = True
                        ' Ｍ２＿合計欄_標準原価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranHyojunGnk).Locked = True
                        ' Ｍ２＿合計欄_粗利額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranArarigaku).Locked = True
                        ' Ｍ２＿合計欄_粗利％
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranArariPar).Locked = True
                        ' Ｍ２＿月額換算後欄_月額費用
                        If Me.rdoNebikiSetteiZidoAnbunRadio.Checked Then
                            If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiGokei) OrElse
                               String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki) Then
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = False
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = True
                            End If
                        Else
                            Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = True
                        End If
                        ' ST#3441横展開 START
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).ImeMode = ImeMode.Disable
                        ' ST#3441横展開 END
                        ' Ｍ２＿月額換算後欄_標準原価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranHyojunGnk).Locked = True
                        ' Ｍ２＿月額換算後欄_粗利額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranArarigaku).Locked = True
                        ' Ｍ２＿月額換算後欄_粗利％
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranArariPar).Locked = True
                    End If
                Next

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM2GokeiIchiran.ResumeLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.DemoKasidasi) OrElse
                   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Sinsei) OrElse
                   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Print) OrElse
                   String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.TanNendoUpdate) Then
                    ' Ｍ１＿アドオン（必須）
                    Me.sprM1MenuIchiran_Sheet1.Columns(buppanEnum.lblM1AddonHissu).Visible = False
                    ' Ｍ１＿アドオン（推奨）
                    Me.sprM1MenuIchiran_Sheet1.Columns(buppanEnum.lblM1AddonSuisho).Visible = False
                End If

            ElseIf String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                ' No1 明細表示設定＿設置先ラベル
                Me.lblMeisaiHyojiSetteiSettisaki.Visible = True
                ' No2 画面＿明細表示設定＿設置先コンボ
                Me.cmbMeisaiHyojiSetteiSettisakiCombo.Enabled = True
                ' No3 保守区分ラベル
                Me.lblHoshuKbn.Visible = True
                ' No4 画面＿保守区分コンボ
                Me.cmbHoshuKbnCombo.Enabled = False
                ' No5 保守料金算出基準日ラベル
                Me.lblHoshuRyokinSansyutsuKijunDate.Enabled = False
                ' No6 画面＿保守料金算出基準日
                Me.sprHoshuRyokinSansyutsuKijunDate.Enabled = False
                ' ST#3441横展開 START
                Me.sprHoshuRyokinSansyutsuKijunDate.ImeMode = ImeMode.Disable
                ' ST#3441横展開 END
                ' No7 契約識別２ラベル
                Me.lblKeiyakuShikibetsu2.Visible = False
                ' No8 画面＿契約識別２
                Me.txtKeiyakuShikibetsu2.Visible = False
                ' No9 保守料コメントラベル
                Me.lblHoshuryoCmt.Visible = False
                ' No10 画面＿保守料コメント
                Me.txtHoshuryoCmt.Visible = False
                ' No11 値引設定ラベル
                Me.lblNebikiSettei.Visible = True
                ' No11 値引設定ラベル２
                Me.lblNebikiSettei2.Visible = True
                ' No12 画面＿値引設定_メニュー別ラジオ
                Me.rdoNebikiSetteiMenuBetsuRadio.Enabled = False
                ' No13 画面＿値引設定_自動按分ラジオ
                Me.rdoNebikiSetteiZidoAnbunRadio.Enabled = False
                ' No14 丸め設定ラベル
                Me.lblMarumeSettei.Visible = True
                ' No15 画面＿値引設定_丸め設定コンボ
                Me.cmbNebikiSetteiMarumeSetteiCombo.Enabled = False
                ' No16 画面＿メニュー選択ボタン
                Me.btnMenuSentaku.Enabled = False
                'ST#8852 START
                Me.RightClickMenuSentaku.Enabled = False
                'ST#8852 END
                ' No17 画面＿推奨構成ボタン
                Me.btnSuishoKosei.Enabled = False
                ' No18 画面＿多拠点ボタン
                Me.btnTaKyoten.Enabled = False
                ' No19 画面＿数・機器変更ボタン
                Me.btnMenuFutai.Enabled = False
                ' No20 画面＿料金再計算ボタン
                Me.btnRyokinSaiKeisan.Enabled = False
                ' No21 画面＿契約付帯入力ボタン
                Me.btnFutaiNyuryoku.Enabled = False
                ' No23 画面＿並び替え▲ボタン
                Me.btnNarabikaeUp.Enabled = False
                ' No24 画面＿並び替えラベル
                Me.lblNarabikae.Visible = True
                ' No25 画面＿並び替え▼ボタン
                Me.btnNarabikaeDown.Enabled = False
                ' No111 画面＿全表示・設定幅ボタン
                Me.btnAllHyojiSetteiHaba.Enabled = True
                ' No112 画面＿幅記憶ボタン
                Me.btnHabaKioku.Enabled = True
                ' No113 合計表示設定ラベル
                Me.lblGokeiHyojiSettei.Visible = True
                ' No114 画面＿合計表示設定_保守ラジオ
                Me.rdoGokeiHyojiSetteiHoshuRadio.Visible = True
                ' No114 画面＿合計表示設定_保守ラジオ
                Me.rdoGokeiHyojiSetteiHoshuRadio.Enabled = True
                ' No115 画面＿合計表示設定_課金ラジオ
                Me.rdoGokeiHyojiSetteiKakinRadio.Visible = True
                ' No115 画面＿合計表示設定_課金ラジオ
                Me.rdoGokeiHyojiSetteiKakinRadio.Enabled = True
                If qc001F04FormDto.SprM1MenuIchiran.Count > 0 Then
                    'ST1#7622 START
                    ' No22 画面＿正誤チェックボタン
                    Me.btnSeigoCheck.Enabled = False
                    'ST1#7622 END
                    ' No116 画面＿明細総合計ボタン
                    Me.btnMeisaiSoGokei.Enabled = True
                Else
                    ' No22 画面＿正誤チェックボタン
                    Me.btnSeigoCheck.Enabled = False
                    ' No116 画面＿明細総合計ボタン
                    Me.btnMeisaiSoGokei.Enabled = False
                End If
                ' No148 画面＿グループ変更ボタン
                Me.btnGroupHenko.Enabled = True
                ' No149 画面＿明細参照ボタン
                Me.btnMeisaiSansyo.Visible = False

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM1MenuIchiran.SuspendLayout()
                'タブ遷移_改善案NO.4-2022.01.17-START

                ' 2021/08/13 #4813 これより先にＳｐｒｅａｄ設定されていればこのままでＯＫ
                'For rowCnt = 0 To Me.sprM1MenuIchiran_Sheet1.RowCount - 1
                For rowCnt = 0 To currentList.Count - 1
                    'ST1_#4936 START
                    For colIdx As Integer = 0 To 31
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, colIdx).BackColor = Drawing.Color.Empty
                    Next

                    'ST#7255
                    If String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ka) Then
                        'ST1_#4936 END
                        If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) Then
                            If String.IsNullOrEmpty(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"), String)) Then
                                sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(204, 255, 255)
                            Else
                                sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                            End If
                        ElseIf String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU2) Then
                            sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(204, 255, 255)
                        Else
                            sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                        End If
                    Else
                        sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    End If

                    ' Ｍ１＿Ｎｏ
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1No).Locked = True
                    ' Ｍ１＿メニュー番号
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).Locked = True
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿メニュー名称
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNm).Locked = True
                    ' Ｍ１＿種別
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Locked = True
                    ' Ｍ１＿契約単位
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1KeiyakuTani).Locked = True
                    ' Ｍ１＿請求
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Locked = True
                    ' Ｍ１＿付帯
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Futai).Locked = True
                    ' Ｍ１＿アドオン（必須）
                    If String.Equals(currentList(rowCnt).M1AddonHissu, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = btnSuisho
                        'ST1#7622 START
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = True
                        'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = False
                        'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).BackColor = Drawing.Color.White
                        'ST1#7622 END
                    End If
                    ' Ｍ１＿アドオン（推奨）
                    If String.Equals(currentList(rowCnt).M1AddonSuisho, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = btnHissu
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = True
                    End If
                    ' Ｍ１＿委託希望
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        If String.Equals(currentList(rowCnt).M1ItakuKibo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Value = False
                        End If
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                    End If
                    ' Ｍ１＿数量
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Sryo).Locked = True
                    ' Ｍ１＿年額定価
                    If String.Equals(currentList(rowCnt).M1NengakuTeika, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Locked = True
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿月額定価
                    If String.Equals(currentList(rowCnt).M1GetsugakuTeika, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).Locked = True
                    ' Ｍ１＿年額売価単価
                    If String.Equals(currentList(rowCnt).M1NengakuBinTnk, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Locked = True
                    'ST#7255 課金の場合、編集不可の項目は色を青くする
                    'If resetBackColor Then
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    'End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿月額売価単価
                    If String.Equals(currentList(rowCnt).M1GetsugakuBinTnk, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Locked = True
                    'ST#7255 課金の場合、編集不可の項目は色を青くする
                    'If resetBackColor Then
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    'End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿年額値引％
                    If String.Equals(currentList(rowCnt).M1NengakuNebikiPar, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).CellType = NumberCellType2
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).Locked = True
                    ' Ｍ１＿月額値引％
                    If String.Equals(currentList(rowCnt).M1GetsugakuNebikiPar, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).CellType = NumberCellType2
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).Locked = True
                    ' Ｍ１＿年額費用
                    If String.Equals(currentList(rowCnt).M1NengakuHiyo, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).Locked = True
                    ' Ｍ１＿月額費用
                    If String.Equals(currentList(rowCnt).M1GetsugakuHiyo, Consts.ousenn) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).Locked = True
                    ' Ｍ１＿無償（初期費用）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        If String.Equals(currentList(rowCnt).M1MusyoShokiHiyo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = False
                        End If
                        '#11373 チェックボックスの値を確定させるため、Focusを再設定する
                        Me.sprM1MenuIchiran.Focus()
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿初期費用
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ShokiHiyo).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ShokiHiyo).Locked = True
                    ' Ｍ１＿無償（随時費用）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        If String.Equals(currentList(rowCnt).M1MusyoZuijiHiyo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = False
                        End If
                        '#11373 横展開 チェックボックスの値を確定させるため、Focusを再設定する
                        Me.sprM1MenuIchiran.Focus()
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿随時費用
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ZuijiHiyo).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ZuijiHiyo).Locked = True
                    ' Ｍ１＿原価区分
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        combox = New CellType.ComboBoxCellType
                        strLst = New List(Of String)
                        If currentList(rowCnt).M1GnkKbn IsNot Nothing AndAlso currentList(rowCnt).M1GnkKbn.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GnkKbn
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                        combox.Items = strLst.ToArray
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).CellType = combox
                        For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GnkKbn
                            If String.Equals(currentList(rowCnt).M1GnkKbnChoose, ComboxOptionDto.Code) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Value = ComboxOptionDto.Name
                            End If
                        Next
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                    End If
                    ' Ｍ１＿標準原価
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = True
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿後粗利
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1AtoArari).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1AtoArari).Locked = True
                    ' Ｍ１＿粗利％
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ArariPar).CellType = NumberCellType2
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ArariPar).Locked = True
                    ' Ｍ１＿月額無償（月数）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        combox = New CellType.ComboBoxCellType
                        strLst = New List(Of String)
                        If currentList(rowCnt).M1GetsugakuMusyoMoNum IsNot Nothing AndAlso currentList(rowCnt).M1GetsugakuMusyoMoNum.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GetsugakuMusyoMoNum
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                        combox.Items = strLst.ToArray
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).CellType = combox
                        If Not String.IsNullOrEmpty(currentList(rowCnt).M1GetsugakuMusyoMoNumChoose) Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GetsugakuMusyoMoNum
                                If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).M1GetsugakuMusyoMoNumChoose) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Value = ComboxOptionDto.Name
                                End If
                            Next
                        End If
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                    End If
                    ' Ｍ１＿設置先コンボ
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).CellType = txt
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = True
                    ' Ｍ１＿グループコンボ
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).CellType = txt
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Locked = True
                    ' Ｍ１＿サブタイトルチェック
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).CellType = chk
                    If currentList(rowCnt).M1SubTtl Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Value = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Value = False
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = True
                    ' Ｍ１＿サブタイトル
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1SubTtl).Locked = True

                Next
                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM1MenuIchiran.ResumeLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM2GokeiIchiran.SuspendLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                For rowCnt = 0 To Me.sprM2GokeiIchiran_Sheet1.RowCount - 1
                    If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).M2hyoujiFlag, Consts.hihyouji) Then
                        Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = False
                    Else
                        If String.IsNullOrEmpty(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2GokeiShbt2) Then
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            Me.sprM2GokeiIchiran_Sheet1.Cells(rowCnt, 0).Border = New FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Lowered, Drawing.Color.Black, Drawing.Color.LightGray, 1)
                            Me.sprM2GokeiIchiran_Sheet1.AddSpanCell(rowCnt, 0, 1, 2)
                        ElseIf String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).TxtM2Bango, qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki) Then
                            If (Me.rdoGokeiHyojiSetteiHoshuRadio.Checked AndAlso String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki)) OrElse
                               (Me.rdoGokeiHyojiSetteiKakinRadio.Checked AndAlso String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.kakinSettisaki)) Then
                                Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            End If
                        End If
                        If Not String.IsNullOrEmpty(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2GokeiShbt2) Then
                            Me.sprM2GokeiIchiran_Sheet1.Cells(rowCnt, 0).Border = New FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Lowered, Drawing.Color.Black, Drawing.Color.LightGray, 1)
                        End If
                        If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisakiNull) OrElse
                           String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.kakinSettisakiNull) Then
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            '契約変更たよ明細タブ合計欄表示不正対応 START
                            'Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).CellType = txt
                            '契約変更たよ明細タブ合計欄表示不正対応 END
                        End If
                        ' Ｍ２＿合計種別
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.sprM2GokeiShbt).Locked = True
                        ' Ｍ２＿合計種別2
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.sprM2GokeiShbt2).Locked = True
                        ' Ｍ２＿合計欄_年額定価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuTeika).Locked = True
                        ' Ｍ２＿合計欄_月額定価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuTeika).Locked = True
                        ' Ｍ２＿合計欄_初期費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranShokiHiyo).Locked = True
                        ' Ｍ２＿合計欄_随時費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranZuijiHiyo).Locked = True
                        ' Ｍ２＿合計欄_年額値引額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuNebikigaku).Locked = True
                        ' Ｍ２＿合計欄_月額値引額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuNebikigaku).Locked = True
                        ' Ｍ２＿合計欄_年額費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = True
                        ' ST#3441横展開 START
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).ImeMode = ImeMode.Disable
                        ' ST#3441横展開 END
                        ' Ｍ２＿合計欄_月額費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuHiyo).Locked = True
                        ' Ｍ２＿合計欄_標準原価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranHyojunGnk).Locked = True
                        ' Ｍ２＿合計欄_粗利額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranArarigaku).Locked = True
                        ' Ｍ２＿合計欄_粗利％
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranArariPar).Locked = True
                        ' Ｍ２＿月額換算後欄_月額費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = True
                        ' ST#3441横展開 START
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).ImeMode = ImeMode.Disable
                        ' ST#3441横展開 END
                        ' Ｍ２＿月額換算後欄_標準原価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranHyojunGnk).Locked = True
                        ' Ｍ２＿月額換算後欄_粗利額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranArarigaku).Locked = True
                        ' Ｍ２＿月額換算後欄_粗利％
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranArariPar).Locked = True
                    End If
                Next

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM2GokeiIchiran.ResumeLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

            Else
                ' 行挿入
                Me.RightClickInsert.Enabled = True
                ' 行削除
                Me.RightClickDelete.Enabled = True
                ' No1 明細表示設定＿設置先ラベル
                Me.lblMeisaiHyojiSetteiSettisaki.Visible = True
                ' No2 画面＿明細表示設定＿設置先コンボ
                Me.cmbMeisaiHyojiSetteiSettisakiCombo.Enabled = True
                ' No3 保守区分ラベル
                Me.lblHoshuKbn.Visible = True
                ' No4 画面＿保守区分コンボ
                Me.cmbHoshuKbnCombo.Enabled = True
                ' No5 保守料金算出基準日ラベル
                Me.lblHoshuRyokinSansyutsuKijunDate.Visible = True
                ' No6 画面＿保守料金算出基準日
                Me.sprHoshuRyokinSansyutsuKijunDate.Visible = True
                ' No6 画面＿保守料金算出基準日
                Me.sprHoshuRyokinSansyutsuKijunDate.Enabled = True
                Me.sprHoshuRyokinSansyutsuKijunDate.TabStop = True
                ' ST#3441横展開 START
                Me.sprHoshuRyokinSansyutsuKijunDate.ImeMode = ImeMode.Disable
                ' ST#3441横展開 END
                ' No7 契約識別２ラベル
                Me.lblKeiyakuShikibetsu2.Visible = False
                ' No8 画面＿契約識別２
                Me.txtKeiyakuShikibetsu2.Visible = False
                ' No9 保守料コメントラベル
                Me.lblHoshuryoCmt.Visible = False
                ' No10 画面＿保守料コメント
                Me.txtHoshuryoCmt.Visible = False
                ' No11 値引設定ラベル
                Me.lblNebikiSettei.Visible = True
                ' No11 値引設定ラベル２
                Me.lblNebikiSettei2.Visible = True
                ' No12 画面＿値引設定_メニュー別ラジオ
                Me.rdoNebikiSetteiMenuBetsuRadio.Enabled = True
                ' No13 画面＿値引設定_自動按分ラジオ
                Me.rdoNebikiSetteiZidoAnbunRadio.Enabled = True
                ' No14 丸め設定ラベル
                Me.lblMarumeSettei.Visible = True
                ' No15 画面＿値引設定_丸め設定コンボ
                If String.Equals(qc001F04FormDto.RdoNebikiSetteiMenuBetsuRadio, Consts.menyuu) Then
                    Me.cmbNebikiSetteiMarumeSetteiCombo.Enabled = False
                Else
                    Me.cmbNebikiSetteiMarumeSetteiCombo.Enabled = True
                End If
                ' No16 画面＿メニュー選択ボタン
                Me.btnMenuSentaku.Enabled = True
                'ST#8852 START
                Me.RightClickMenuSentaku.Enabled = True
                'ST#8852 END
                ' No17 画面＿推奨構成ボタン
                Me.btnSuishoKosei.Enabled = True
                ' No23 画面＿並び替え▲ボタン
                Me.btnNarabikaeUp.Enabled = True
                ' No24 画面＿並び替えラベル
                Me.lblNarabikae.Visible = True
                ' No25 画面＿並び替え▼ボタン
                Me.btnNarabikaeDown.Enabled = True
                ' No111 画面＿全表示・設定幅ボタン
                Me.btnAllHyojiSetteiHaba.Enabled = True
                ' No112 画面＿幅記憶ボタン
                Me.btnHabaKioku.Enabled = True
                ' No113 合計表示設定ラベル
                Me.lblGokeiHyojiSettei.Visible = True
                ' No114 画面＿合計表示設定_保守ラジオ
                Me.rdoGokeiHyojiSetteiHoshuRadio.Visible = True
                ' No114 画面＿合計表示設定_保守ラジオ
                Me.rdoGokeiHyojiSetteiHoshuRadio.Enabled = True
                ' No115 画面＿合計表示設定_課金ラジオ
                Me.rdoGokeiHyojiSetteiKakinRadio.Visible = True
                ' No115 画面＿合計表示設定_課金ラジオ
                Me.rdoGokeiHyojiSetteiKakinRadio.Enabled = True
                If qc001F04FormDto.SprM1MenuIchiran.Count > 1 Then
                    ' No18 画面＿多拠点ボタン
                    Me.btnTaKyoten.Enabled = True
                    ' No22 画面＿正誤チェックボタン
                    Me.btnSeigoCheck.Enabled = True
                    ' No116 画面＿明細総合計ボタン
                    Me.btnMeisaiSoGokei.Enabled = True
                    ' #8005
                    ' No19 画面＿数・機器変更ボタン
                    Me.btnMenuFutai.Enabled = True
                    ' No20 画面＿料金再計算ボタン
                    Me.btnRyokinSaiKeisan.Enabled = True
                    ' No21 画面＿契約付帯入力ボタン
                    '2022.10.07 #13563 MOD-START　付帯入力ボタン制御条件の修正
                    '#12237 20220909 MOD-START　付帯入力ボタン非活性・活性るため追加
                    'If hissuflag Then
                    '    If qc001F04FormDto.CmbM1SettisakiCombo.Count > 1 Then
                    '        Me.btnFutaiNyuryoku.Enabled = True
                    '    Else
                    '        Me.btnFutaiNyuryoku.Enabled = False
                    '    End If
                    'ElseIf Not hissuflag Then
                    '    Me.btnFutaiNyuryoku.Enabled = True
                    'End If
                    '#12237 20220909 MOD-END　付帯入力ボタン非活性・活性るため追加
                    Me.btnFutaiNyuryoku.Enabled = hissuflag
                    '2022.10.07 #13563 MOD-END　付帯入力ボタン制御条件の修正
                Else
                    ' No18 画面＿多拠点ボタン
                    Me.btnTaKyoten.Enabled = False
                    ' No22 画面＿正誤チェックボタン
                    Me.btnSeigoCheck.Enabled = False
                    ' No116 画面＿明細総合計ボタン
                    Me.btnMeisaiSoGokei.Enabled = False
                    ' #8005
                    ' No19 画面＿数・機器変更ボタン
                    Me.btnMenuFutai.Enabled = False
                    ' No20 画面＿料金再計算ボタン
                    Me.btnRyokinSaiKeisan.Enabled = False
                    ' No21 画面＿契約付帯入力ボタン
                    Me.btnFutaiNyuryoku.Enabled = False
                End If
                ' No117 Ｍ２＿合計一覧
                Me.sprM2GokeiIchiran.Enabled = True
                ' No148 画面＿グループ変更ボタン
                Me.btnGroupHenko.Enabled = True
                ' No149 画面＿明細参照ボタン
                Me.btnMeisaiSansyo.Visible = False

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM1MenuIchiran.SuspendLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                ' 2021/08/13 #4813 これより先にＳｐｒｅａｄ設定されていればこのままでＯＫ
                'For rowCnt = 0 To Me.sprM1MenuIchiran_Sheet1.RowCount - 1
                For rowCnt = 0 To currentList.Count - 1

                    'ST1_#4936 START
                    For colIdx As Integer = 0 To 31
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, colIdx).BackColor = Drawing.Color.Empty
                    Next

                    'ST#7255
                    If String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ka) Then
                        'ST1_#4936 END
                        If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) Then
                            If String.IsNullOrEmpty(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"), String)) Then
                                sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(204, 255, 255)
                            Else
                                sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                            End If
                        ElseIf String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU2) Then
                            sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(204, 255, 255)
                        Else
                            sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                        End If
                    Else
                        sprM1MenuIchiran_Sheet1.Rows(rowCnt).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                    End If

                    ' Ｍ１＿Ｎｏ
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1No).Locked = True
                    ' Ｍ１＿メニュー番号
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).Locked = False
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).BackColor = Drawing.Color.White
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿メニュー名称
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1MenuNm).Locked = True
                    ' Ｍ１＿種別
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Locked = True
                    ' Ｍ１＿契約単位
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1KeiyakuTani).Locked = True
                    ' Ｍ１＿請求
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Locked = True
                    ' Ｍ１＿付帯
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Futai).Locked = True
                    ' Ｍ１＿アドオン（必須）
                    If String.Equals(currentList(rowCnt).M1AddonHissu, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = True
                    Else
                        ' 障害_ST先行検証 #10662 Start
                        'If (String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ho) AndAlso
                        '    Me.rdoNebikiSetteiZidoAnbunRadio.Checked) Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = btnSuisho
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = True
                        'Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).CellType = btnSuisho
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonHissu).BackColor = Drawing.Color.White
                        'End If
                        ' 障害_ST先行検証 #10662 End
                    End If
                    ' Ｍ１＿アドオン（推奨）
                    If String.Equals(currentList(rowCnt).M1AddonSuisho, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = True
                    Else
                        ' 障害_ST先行検証 #10662 Start
                        'If (String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ho) AndAlso
                        '   Me.rdoNebikiSetteiZidoAnbunRadio.Checked) Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = btnHissu
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = True
                        'Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).CellType = btnHissu
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1AddonSuisho).BackColor = Drawing.Color.White
                        'End If
                        ' 障害_ST先行検証 #10662 End
                    End If
                    ' Ｍ１＿委託希望
                    If String.Equals(qc001F04FormDto.RdoNebikiSetteiMenuBetsuRadio, Consts.jidoubunsuu) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                    Else
                        If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                            If String.Equals(currentList(rowCnt).M1ItakuKibo, Consts.checKbox.checktrue) Then
                                sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = chk
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Value = True
                            Else
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = chk
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Value = False
                            End If
                            If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailComDto, "GROUPNO"), Consts.GROUPNO.ari) AndAlso
                           String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailHosDto, "PACK345_FLG"), Consts.PACK345FLG.PACK345iie) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = False
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                            End If
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).CellType = txt
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Text = Consts.ousenn
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ItakuKibo).Locked = True
                        End If
                    End If
                    ' Ｍ１＿数量
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Sryo).Locked = True
                    ' Ｍ１＿年額定価
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).CellType = NumberCellType1
                    If String.Equals(currentList(rowCnt).M1NengakuTeika, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Locked = True
                    ElseIf Me.rdoNebikiSetteiMenuBetsuRadio.Checked AndAlso
                       String.Equals(currentList(rowCnt).M1ItakuKibo, Consts.checKbox.checktrue) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Locked = False

                        '20220805 ST#12408 DEL-START
                        'If resetBackColor Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).BackColor = Drawing.Color.White
                        'End If
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).BackColor = Drawing.Color.White
                        '20220805 ST#12408 DEL-END
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuTeika).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿月額定価
                    If String.Equals(currentList(rowCnt).M1GetsugakuTeika, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuTeika).Locked = True
                    ' Ｍ１＿年額売価単価
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).CellType = NumberCellType1
                    If String.Equals(currentList(rowCnt).M1NengakuBinTnk, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Locked = True
                        'ST#7255 課金の場合、編集不可の項目は色を青くする
                        'If resetBackColor Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                        'End If
                        ' #8260 T)Annaka Start
                        'ElseIf (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                        '        Me.rdoNebikiSetteiMenuBetsuRadio.Checked) OrElse
                        '       (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                        '        String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Value, Consts.nen)) Then
                    ElseIf (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                            Me.rdoNebikiSetteiMenuBetsuRadio.Checked) OrElse
                           (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                            String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Value, Consts.nen) AndAlso
                            String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailComDto, "AK_MENUNEBIKI_FLG"), Consts.ari)) Then
                        ' #8260 T)Annaka End
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Locked = False
                        ' #9196 START
                        'If resetBackColor Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).BackColor = Drawing.Color.White
                        'End If
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).BackColor = Drawing.Color.White
                        ' #9196 END
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).Locked = True
                        'ST#7255 課金の場合、編集不可の項目は色を青くする
                        'If resetBackColor Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                        'End If
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuBinTnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿月額売価単価
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).CellType = NumberCellType1
                    If String.Equals(currentList(rowCnt).M1GetsugakuBinTnk, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Locked = True
                        'ST#7255 課金の場合、編集不可の項目は色を青くする
                        'If resetBackColor Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                        'End If
                        ' #8260 T)Annaka Start
                        'ElseIf (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                        '        Me.rdoNebikiSetteiMenuBetsuRadio.Checked) OrElse
                        '       (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                        '        String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Value, Consts.tsuki)) Then
                    ElseIf (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                            Me.rdoNebikiSetteiMenuBetsuRadio.Checked) OrElse
                           (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                            String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1Seikyu).Value, Consts.tsuki) AndAlso
                            String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailComDto, "AK_MENUNEBIKI_FLG"), Consts.ari)) Then
                        ' #8260 T)Annaka End
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Locked = False
                        ' #9196 START
                        'If resetBackColor Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).BackColor = Drawing.Color.White
                        'End If
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).BackColor = Drawing.Color.White
                        ' #9196 END
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).Locked = True
                        'ST#7255 課金の場合、編集不可の項目は色を青くする
                        'If resetBackColor Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).BackColor = Drawing.Color.FromArgb(240, 240, 240)
                        'End If
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuBinTnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿年額値引％
                    If String.Equals(currentList(rowCnt).M1NengakuNebikiPar, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).CellType = NumberCellType2
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1NengakuNebikiPar).Locked = True
                    ' Ｍ１＿月額値引％
                    If String.Equals(currentList(rowCnt).M1GetsugakuNebikiPar, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).CellType = NumberCellType2
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1GetsugakuNebikiPar).Locked = True
                    ' Ｍ１＿年額費用
                    If String.Equals(currentList(rowCnt).M1NengakuHiyo, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1NengakuHiyo).Locked = True
                    ' Ｍ１＿月額費用
                    If String.Equals(currentList(rowCnt).M1GetsugakuHiyo, Consts.ousenn) OrElse
                       String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).Text = Consts.ousenn
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).CellType = NumberCellType1
                    End If
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuHiyo).Locked = True
                    ' Ｍ１＿無償（初期費用）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        If String.Equals(currentList(rowCnt).M1MusyoShokiHiyo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = False
                        End If
                        '#11373 チェックボックスの値を確定させるため、Focusを再設定する
                        '#12150 2022.08.08 START
                        'Me.sprM1MenuIchiran.Focus()
                        If Not Me.bFlag Then
                            Me.sprM1MenuIchiran.Focus()
                        End If
                        '#12150 2022.08.08 END
                        'ST1_2811 STRAT
                        If CDec(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "SYOKI_HIYO")) = 0 Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                        Else
                            If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN2) OrElse
                                String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN3) OrElse
                                (String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN4) AndAlso
                                qc001F04FormDto.syoriRes = 0) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = False
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).BackColor = Drawing.Color.White
                            Else
                                '#11450 2022.08.27 MOD START 無償（初期費用）チェック外す時メッセージ出力追加
                                '#11450 2022.07.17 ADD START 無償（初期費用）チェック外す対応
                                'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = chk
                                'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = False
                                '#11450 2022.07.17 ADD END 無償（初期費用）チェック外す対応
                                If String.Equals(currentList(rowCnt).M1MusyoShokiHiyo, Consts.checKbox.checktrue) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Value = False
                                    '20220906 #13087 ADD START 無償（初期費用）チェック外す対応
                                    Dim actionName As String = GetType(QC001F04Action).AssemblyQualifiedName
                                    Dim actionMethodName As String = "LblM1MusyoShokiHiyo_Unchecked"
                                    UpdateProcessingFlagToFalse() '２回目以降のAction実行は処理中フラグをOFFにさせる
                                    If qc001F04FormDto.SelectedRowIndex.Count = 0 Then
                                        qc001F04FormDto.SelectedRowIndex.Add(rowCnt)
                                    Else
                                        qc001F04FormDto.SelectedRowIndex(0) = rowCnt
                                    End If
                                    qc001F04FormDto = CType(ExecuteAction(actionName, actionMethodName, qc001F04FormDto, Nothing), QC001F04FormDto)
                                    Me.sprM2GokeiIchiran_Sheet1.DataSource = qc001F04FormDto.SprM2GokeiIchiran
                                    '20220906 #13087 ADD END
                                    '2022.09.17 #13409 ADD-START
                                    'MessageDialogUtil.ShowInfo(GetDialogProperty(BusinessMessageConst.IKB031, currentList(rowCnt).M1MenuNo))
                                    Dim checkSeigoDto = DirectCast(ApplicationScope.InstanceData.GetValue("CheckAllSeigoDto"), QC001F00CheckSeigoDTO)
                                    If Not IsNothing(checkSeigoDto) AndAlso checkSeigoDto.MessageInfoDtoList IsNot Nothing AndAlso checkSeigoDto.MessageInfoDtoList.Count > 0 Then
                                        If Not checkSeigoDto.MsgIdList.Contains(BusinessMessageConst.IKB031) Then
                                            MessageDialogUtil.ShowInfo(GetDialogProperty(BusinessMessageConst.IKB031, currentList(rowCnt).M1MenuNo))
                                        End If
                                        '#13409 2022-10-22 Add Start
                                    Else
                                        MessageDialogUtil.ShowInfo(GetDialogProperty(BusinessMessageConst.IKB031, currentList(rowCnt).M1MenuNo))
                                        '#13409 2022-10-22 Add End
                                    End If
                                    '2022.09.17 #13409 ADD-END
                                End If
                                '#11450 2022.08.27 MOD END 
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                            End If
                        End If
                        'ST1_2811 END
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoShokiHiyo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿初期費用
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ShokiHiyo).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ShokiHiyo).Locked = True
                    ' Ｍ１＿無償（随時費用）
                    '2022.11.11 MOD-START #14596 チェックボックス非活性制御修正
                    ''4784 対応
                    'If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso qc001F04FormDto.TuikaMJTAMNUDtoList IsNot Nothing AndAlso qc001F04FormDto.TuikaMJTAMNUDtoList.Count > rowCnt AndAlso Not String.Equals(GetValueDic(CType(GetValueDic(qc001F04FormDto.TuikaMJTAMNUDtoList(rowCnt), "QC001S04MstDetailKakinDTO"), Dictionary(Of String, Object)), "ZUIJI_MUSYO_KBN"), Consts.musyounai) Then
                    '    If String.Equals(currentList(rowCnt).M1MusyoZuijiHiyo, Consts.checKbox.checktrue) Then
                    '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                    '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = True
                    '    Else
                    '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                    '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = False
                    '    End If
                    '    '#11373 横展開 チェックボックスの値を確定させるため、Focusを再設定する
                    '    Me.sprM1MenuIchiran.Focus()
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = False
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).BackColor = Drawing.Color.White
                    'ElseIf String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                    '    'IT-#565 START
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                    '    If Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value Is Nothing Then
                    '        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = False
                    '    End If
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = False
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).BackColor = Drawing.Color.White
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        If String.Equals(currentList(rowCnt).M1MusyoZuijiHiyo, Consts.checKbox.checktrue) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = True
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = chk
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = False
                        End If

                        If Not IsNothing(qc001F04FormDto.TuikaMJTAMNUDtoList) AndAlso
                           qc001F04FormDto.TuikaMJTAMNUDtoList.Count > rowCnt AndAlso
                           Not String.Equals(GetValueDic(CType(GetValueDic(qc001F04FormDto.TuikaMJTAMNUDtoList(rowCnt), "QC001S04MstDetailKakinDTO"),
                                                         Dictionary(Of String, Object)), "ZUIJI_MUSYO_KBN"), Consts.musyounai) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).BackColor = Drawing.Color.White
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = True
                        End If
                        '2022.11.11 MOD-END #14596 チェックボックス非活性制御修正
                        '#13409 2022-10-22 Add Start
                        If Not (String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN2) OrElse
                            String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN3) OrElse
                            (String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN4) AndAlso
                              qc001F04FormDto.syoriRes = 0)) Then

                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Value = Consts.checKbox.checkfalse
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).BackColor = Drawing.Color.FromArgb(153, 204, 255)
                        End If
                        '#13409 2022-10-22 Add End
                        '#11373 横展開 チェックボックスの値を確定させるため、Focusを再設定する
                        '#12150 2022.08.08 START
                        'Me.sprM1MenuIchiran.Focus()
                        If Not Me.bFlag Then
                            Me.sprM1MenuIchiran.Focus()
                        End If
                        '#12150 2022.08.08 END
                        'IT-#565 END
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1MusyoZuijiHiyo).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿随時費用
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ZuijiHiyo).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ZuijiHiyo).Locked = True
                    ' Ｍ１＿原価区分
                    'ST1_#4801 START
                    'If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) OrElse
                        (String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso currentList(rowCnt).M1GnkKbn.Count > 0) Then
                        'ST1_#4801 END
                        combox = New CellType.ComboBoxCellType
                        strLst = New List(Of String)
                        If currentList(rowCnt).M1GnkKbn IsNot Nothing AndAlso currentList(rowCnt).M1GnkKbn.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GnkKbn
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                        combox.Items = strLst.ToArray
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).CellType = combox
                        For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GnkKbn
                            If String.Equals(currentList(rowCnt).M1GnkKbnChoose, ComboxOptionDto.Code) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Value = ComboxOptionDto.Name
                            End If
                        Next
                        If Me.rdoNebikiSetteiMenuBetsuRadio.Checked Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).BackColor = Drawing.Color.White
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GnkKbn).Locked = True
                    End If
                    ' Ｍ１＿標準原価
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).CellType = NumberCellType1
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                    String.Equals(currentList(rowCnt).KOBETUFLG, Consts.KOBETUFLG) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).BackColor = Drawing.Color.White
                        'ST1_#4801 START
                    ElseIf String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) AndAlso
                        String.Equals(Trim(currentList(rowCnt).M1GnkKbnChoose), "Z") Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).BackColor = Drawing.Color.White
                        'ST1_#4801 END
                    ElseIf String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) AndAlso
                       String.Equals(currentList(rowCnt).GENCALCKBN, Consts.GENCALCKBN.GENCALCKBN_9) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = False
                        If resetBackColor Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).BackColor = Drawing.Color.White
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).Locked = True
                    End If
                    ' ST#3441横展開 START
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1HyojunGnk).ImeMode = ImeMode.Disable
                    ' ST#3441横展開 END
                    ' Ｍ１＿後粗利
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1AtoArari).CellType = NumberCellType1
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1AtoArari).Locked = True
                    ' Ｍ１＿粗利％
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ArariPar).CellType = NumberCellType2
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1ArariPar).Locked = True
                    ' Ｍ１＿月額無償（月数）
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ka) Then
                        combox = New CellType.ComboBoxCellType
                        strLst = New List(Of String)
                        If currentList(rowCnt).M1GetsugakuMusyoMoNum IsNot Nothing AndAlso currentList(rowCnt).M1GetsugakuMusyoMoNum.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GetsugakuMusyoMoNum
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                        combox.Items = strLst.ToArray
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).CellType = combox
                        If Not String.IsNullOrEmpty(currentList(rowCnt).M1GetsugakuMusyoMoNumChoose) Then
                            For Each ComboxOptionDto As ComboxOptionDto In currentList(rowCnt).M1GetsugakuMusyoMoNum
                                If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).M1GetsugakuMusyoMoNumChoose) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Value = ComboxOptionDto.Name
                                End If
                            Next
                        End If
                        If String.Equals(strLst.Count, 0) Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                        ElseIf strLst.Count = 1 Then
                            If (String.Equals(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), String), Consts.MUSYODISPKBN.MUSYODISPKBN2) OrElse
                                String.Equals(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), String), Consts.MUSYODISPKBN.MUSYODISPKBN3) OrElse
                                String.Equals(CType(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), String), Consts.MUSYODISPKBN.MUSYODISPKBN4)) AndAlso
                                String.Equals(strLst(0), "0") Then
                                '#13409 2022-10-22 Add Start
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Value = "0"
                                '#13409 2022-10-22 Add End
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                            End If
                        Else
                            'ST1_2811 START
                            If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN2) OrElse
                                    String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN3) OrElse
                                    (String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "MUSYO_DISP_KBN"), Consts.MUSYODISPKBN.MUSYODISPKBN4) AndAlso
                                    qc001F04FormDto.syoriRes = 0) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = False
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).BackColor = Drawing.Color.White
                            Else
                                '#13409 2022-10-22 Add Start
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Value = "0"
                                '#13409 2022-10-22 Add End
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                            End If
                            'ST1_2811 END
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).CellType = txt
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Text = Consts.ousenn
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1GetsugakuMusyoMoNum).Locked = True
                    End If
                    ' Ｍ１＿設置先コンボ
                    combox = New CellType.ComboBoxCellType
                    strLst = New List(Of String)
                    If qc001F04FormDto.CmbM1SettisakiCombo IsNot Nothing AndAlso qc001F04FormDto.CmbM1SettisakiCombo.Count > 0 Then
                        For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.CmbM1SettisakiCombo
                            strLst.Add(ComboxOptionDto.Name)
                        Next
                    End If
                    combox.Items = strLst.ToArray
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).CellType = combox
                    If Not String.IsNullOrEmpty(currentList(rowCnt).CmbM1SettisakiComboCode) Then
                        For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.CmbM1SettisakiCombo
                            If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).CmbM1SettisakiComboCode) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Value = ComboxOptionDto.Name
                            End If
                        Next
                    End If
                    ' ST#6907 START
                    ' 障害_ST先行検証 #10662 IT #213 Start
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                    '#IT1-1015 変更 Start
                    If String.IsNullOrWhiteSpace(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = True
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.LightGray
                    End If
                    '#IT1-1015 変更 End

                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        'If Me.rdoNebikiSetteiZidoAnbunRadio.Checked Then
                        '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = True
                        'Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                        'End If
                    ElseIf String.Equals(currentList(rowCnt).M1Syubetu, Consts.M1Syubetu.ka) Then
                        If Not String.IsNullOrWhiteSpace(currentList(rowCnt).SerMenuno) Then
                            'If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) OrElse
                            'String.Equals(currentList(rowCnt).SerMenuno, currentList(rowCnt).M1NaibuNo) Then

                            'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                            'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                            'Else
                            'Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = True
                            'End If
                            '2022.09.14 #13321 MOD-START
                            If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU0) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                            ElseIf (String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) AndAlso
                                IsNothing(GetValueDic(currentList(rowCnt).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"))) Then
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = True
                            Else
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                                Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                            End If
                            '2022.09.15 #13321 MOD-END
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1SettisakiCombo).BackColor = Drawing.Color.White
                        End If
                    End If
                    'End If
                    ' 障害_ST先行検証 #10662 IT #213 End
                    ' ST#6907 END
                    ' Ｍ１＿グループコンボ
                    combox = New CellType.ComboBoxCellType
                    strLst = New List(Of String)
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        If qc001F04FormDto.HoxyuCmbM1GroupCombo IsNot Nothing Then
                            For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.HoxyuCmbM1GroupCombo
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                    Else
                        If qc001F04FormDto.KakinCmbM1GroupCombo IsNot Nothing AndAlso qc001F04FormDto.KakinCmbM1GroupCombo.Count > 0 Then
                            For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.KakinCmbM1GroupCombo
                                strLst.Add(ComboxOptionDto.Name)
                            Next
                        End If
                    End If
                    combox.Items = strLst.ToArray
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).CellType = combox
                    If Not String.IsNullOrEmpty(currentList(rowCnt).CmbM1GroupComboCode) Then
                        If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                            For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.HoxyuCmbM1GroupCombo
                                If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).CmbM1GroupComboCode) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Value = ComboxOptionDto.Name
                                End If
                            Next
                        Else
                            For Each ComboxOptionDto As ComboxOptionDto In qc001F04FormDto.KakinCmbM1GroupCombo
                                If String.Equals(ComboxOptionDto.Code, currentList(rowCnt).CmbM1GroupComboCode) Then
                                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Value = ComboxOptionDto.Name
                                End If
                            Next
                        End If
                    End If
                    ' 障害_ST先行検証 #10662 IT #213 Start
                    'If (Me.rdoNebikiSetteiZidoAnbunRadio.Checked AndAlso
                    '   String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho)) OrElse
                    '   String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                    '    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Locked = True
                    'Else
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Locked = False
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).BackColor = Drawing.Color.White

                    '#IT1-1015 変更 Start
                    If String.IsNullOrEmpty(currentList(rowCnt).M1MenuNo) Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).Locked = True
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.cmbM1GroupCombo).BackColor = Drawing.Color.LightGray
                    End If
                    '#IT1-1015 変更 End

                    'End If
                    ' 障害_ST先行検証 #10662 IT #213 End
                    ' Ｍ１＿サブタイトルチェック
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).CellType = chk
                    If currentList(rowCnt).M1SubTtl Then
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Value = True
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Value = False
                    End If
                    If String.Equals(Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.txtM1Syubetu).Value, Consts.M1Syubetu.ho) Then
                        If String.Equals(GetValueDic(currentList(rowCnt).QC001S04MstDetailComDto, "SUBTITLEKBN"), Consts.SUBTITLEKBN.SUBTITLEKBN1) AndAlso
                           Me.rdoNebikiSetteiMenuBetsuRadio.Checked Then
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = False
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).BackColor = Drawing.Color.White
                        Else
                            Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = True
                        End If
                    Else
                        Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.M1SubTtl).Locked = True
                    End If
                    ' Ｍ１＿サブタイトル
                    Me.sprM1MenuIchiran_Sheet1.Cells.Item(rowCnt, buppanEnum.lblM1SubTtl).Locked = True

                Next


                '#12150 2022.08.08 START
                Me.bFlag = False
                '#12150 2022.08.08 END

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM1MenuIchiran.ResumeLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM2GokeiIchiran.SuspendLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

                For rowCnt = 0 To Me.sprM2GokeiIchiran_Sheet1.RowCount - 1
                    If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).M2hyoujiFlag, Consts.hihyouji) Then
                        Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = False
                    Else
                        If String.IsNullOrEmpty(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2GokeiShbt2) Then
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            Me.sprM2GokeiIchiran_Sheet1.Cells(rowCnt, 0).Border = New FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Lowered, Drawing.Color.Black, Drawing.Color.LightGray, 1)
                            Me.sprM2GokeiIchiran_Sheet1.AddSpanCell(rowCnt, 0, 1, 2)
                        ElseIf String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).TxtM2Bango, qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki) Then
                            If (Me.rdoGokeiHyojiSetteiHoshuRadio.Checked AndAlso String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki)) OrElse
                               (Me.rdoGokeiHyojiSetteiKakinRadio.Checked AndAlso String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.kakinSettisaki)) Then
                                Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            End If
                        End If
                        If Not String.IsNullOrEmpty(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2GokeiShbt2) Then
                            Me.sprM2GokeiIchiran_Sheet1.Cells(rowCnt, 0).Border = New FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Lowered, Drawing.Color.Black, Drawing.Color.LightGray, 1)
                        End If
                        If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisakiNull) OrElse
                           String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.kakinSettisakiNull) Then
                            Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).Visible = True
                            '契約変更たよ明細タブ合計欄表示不正対応 START
                            'Me.sprM2GokeiIchiran_Sheet1.Rows(rowCnt).CellType = txt
                            '契約変更たよ明細タブ合計欄表示不正対応 END
                        End If
                        ' Ｍ２＿合計種別
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.sprM2GokeiShbt).Locked = True
                        ' Ｍ２＿合計種別2
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.sprM2GokeiShbt2).Locked = True
                        ' Ｍ２＿合計欄_年額定価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuTeika).Locked = True
                        ' Ｍ２＿合計欄_月額定価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuTeika).Locked = True
                        ' Ｍ２＿合計欄_初期費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranShokiHiyo).Locked = True
                        ' Ｍ２＿合計欄_随時費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranZuijiHiyo).Locked = True
                        ' Ｍ２＿合計欄_年額値引額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuNebikigaku).Locked = True
                        ' Ｍ２＿合計欄_月額値引額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuNebikigaku).Locked = True
                        ' Ｍ２＿合計欄_年額費用
                        If Me.rdoNebikiSetteiZidoAnbunRadio.Checked Then
                            If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiGokei) OrElse
                           String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki) Then
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = False
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = True
                            End If
                        Else
                            Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).Locked = True
                        End If
                        ' ST#3441横展開 START
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranNengakuHiyo).ImeMode = ImeMode.Disable
                        ' ST#3441横展開 END
                        ' Ｍ２＿合計欄_月額費用
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranGetsugakuHiyo).Locked = True
                        ' Ｍ２＿合計欄_標準原価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranHyojunGnk).Locked = True
                        ' Ｍ２＿合計欄_粗利額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranArarigaku).Locked = True
                        ' Ｍ２＿合計欄_粗利％
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GokeiranArariPar).Locked = True
                        ' Ｍ２＿月額換算後欄_月額費用
                        If String.Equals(Me.rdoNebikiSetteiZidoAnbunRadio.Checked, True) Then
                            If String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiGokei) OrElse
                           String.Equals(qc001F04FormDto.SprM2GokeiIchiran(rowCnt).SprM2NaibuGokeiShbt, Consts.GokeiShbt.hoshiSettisaki) Then
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = False
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).BackColor = Drawing.Color.White
                            Else
                                Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = True
                            End If
                        Else
                            Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).Locked = True
                        End If
                        ' ST#3441横展開 START
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranGetsugakuHiyo).ImeMode = ImeMode.Disable
                        ' ST#3441横展開 END
                        ' Ｍ２＿月額換算後欄_標準原価
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranHyojunGnk).Locked = True
                        ' Ｍ２＿月額換算後欄_粗利額
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranArarigaku).Locked = True
                        ' Ｍ２＿月額換算後欄_粗利％
                        Me.sprM2GokeiIchiran_Sheet1.Cells.Item(rowCnt, buppanGokeiEnum.lblM2GetsugakuKansangoranArariPar).Locked = True
                    End If
                Next

                'タブ遷移_改善案NO.4-2022.01.17-START
                Me.sprM2GokeiIchiran.ResumeLayout()
                'タブ遷移_改善案NO.4-2022.01.17-END

            End If

            '6106 Start
            If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Change) Then
                ' 画面＿保守区分コンボ
                Me.cmbHoshuKbnCombo.Enabled = False
            End If
            '6106 End

            ' DAS経由でログインした場合
            If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.DasFlg, Consts.DasMode) Then
                ' Ｍ１＿標準原価
                Me.sprM1MenuIchiran_Sheet1.Columns(buppanEnum.txtM1HyojunGnk).Visible = False
                ' Ｍ１＿後粗利
                Me.sprM1MenuIchiran_Sheet1.Columns(buppanEnum.txtM1AtoArari).Visible = False
                ' Ｍ１＿粗利％
                Me.sprM1MenuIchiran_Sheet1.Columns(buppanEnum.lblM1ArariPar).Visible = False
                ' Ｍ２＿合計欄_標準原価
                Me.sprM2GokeiIchiran_Sheet1.Columns(buppanGokeiEnum.lblM2GokeiranHyojunGnk).Visible = False
                ' Ｍ２＿合計欄_粗利額
                Me.sprM2GokeiIchiran_Sheet1.Columns(buppanGokeiEnum.lblM2GokeiranArarigaku).Visible = False
                ' Ｍ２＿合計欄_粗利％
                Me.sprM2GokeiIchiran_Sheet1.Columns(buppanGokeiEnum.lblM2GokeiranArariPar).Visible = False
                ' Ｍ２＿月額換算後欄_標準原価
                Me.sprM2GokeiIchiran_Sheet1.Columns(buppanGokeiEnum.lblM2GetsugakuKansangoranHyojunGnk).Visible = False
                ' Ｍ２＿月額換算後欄_粗利額
                Me.sprM2GokeiIchiran_Sheet1.Columns(buppanGokeiEnum.lblM2GetsugakuKansangoranArarigaku).Visible = False
                ' Ｍ２＿月額換算後欄_粗利％
                Me.sprM2GokeiIchiran_Sheet1.Columns(buppanGokeiEnum.lblM2GetsugakuKansangoranArariPar).Visible = False
            End If

            '#4401 START
            '参照モード以外の場合
            If Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                '画面＿並び替え▲ボタン
                If qc001F04FormDto.SprM1MenuIchiran.Count > 1 Then
                    '#6144
                    If sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow >= 0 Then
                        ' 選択行の行番号取得
                        Dim selectRowIndex = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)

                        If selectRowIndex = 0 Then
                            btnNarabikaeUp.Enabled = False
                        Else
                            btnNarabikaeUp.Enabled = True
                        End If
                    Else
                        btnNarabikaeUp.Enabled = False
                    End If
                Else
                    btnNarabikaeUp.Enabled = False
                End If

                '画面＿並び替え▼ボタン
                If qc001F04FormDto.SprM1MenuIchiran.Count > 1 Then
                    '#6144
                    If sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow >= 0 Then
                        ' 選択行の行番号取得
                        '#6144
                        Dim selectRowIndex = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)

                        ' 選択行の行数を取得する
                        '#6144
                        Dim selectRowCnt = sprM1MenuIchiran_Sheet1.Models.Selection.LeadRow - sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow + 1

                        '最後の行を選択する場合
                        '#4902
                        If selectRowIndex + selectRowCnt = qc001F04FormDto.SprM1MenuIchiran.Count - 1 Then
                            btnNarabikaeDown.Enabled = False
                        Else
                            btnNarabikaeDown.Enabled = True
                        End If
                    End If
                Else
                    btnNarabikaeDown.Enabled = False
                End If
            End If

            Me.changedFlg = True

            ' 固定必須入力項目
            Dim requiredFields = New Dictionary(Of Control, Control)

            '保守料金算出基準日
            requiredFields.Add(Me.sprHoshuRyokinSansyutsuKijunDate, Me.lblHoshuRyokinSansyutsuKijunDate)

            '基底クラスへ必須項目を設定する
            Me.InitRequiredFieldMarks(requiredFields)

            '必須マークを付ける
            '保守料金算出基準日
            Me.SetRequiredFieldMark(New List(Of Control) From {Me.sprHoshuRyokinSansyutsuKijunDate})


            '#11622 参照モードのコントロール制御を追加 Start
            If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnSyoriMode, Consts.SyoriMode.Reference) Then
                Me.InitControlReference()
            End If
            '#11622 参照モードのコントロール制御を追加 End

            ClientLogUtil.Logger.DebugAP("QC001F04Form:ControlGamenKomoku end")
        End Sub

        ''' <summary>
        ''' Spread初期化
        ''' </summary>
        Private Sub InitSpread()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:InitSpread start")
            '選択範囲のクリア
            sprM1MenuIchiran.ActiveSheet.ClearSelection()

            ' Ｍ２列ヘッダの設定
            sprM2GokeiIchiran_Sheet1.ColumnHeader.RowCount = 2
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(0, 0).Value = ""
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(0, 2).Value = "合計欄"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(0, 13).Value = "月額換算後欄"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 2).Value = "年額定価"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 3).Value = "値引額"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 4).Value = "年額費用"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 5).Value = "月額定価"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 6).Value = "値引額"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 7).Value = "月額費用"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 8).Value = "初期費用"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 9).Value = "随時費用"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 10).Value = "標準原価"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 11).Value = "粗利額"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 12).Value = "粗利％"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 13).Value = "月額費用"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 14).Value = "標準原価"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 15).Value = "粗利額"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(1, 16).Value = "粗利％"
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(0, 0).RowSpan = 2
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(0, 0).ColumnSpan = 2
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(0, 2).ColumnSpan = 11
            sprM2GokeiIchiran_Sheet1.ColumnHeader.Cells(0, 13).ColumnSpan = 4

            '画面共通項目の設定を行う
            'カーソル設定
            sprM1MenuIchiran.SetCursor(CursorType.Normal, Cursors.Default)
            sprM2GokeiIchiran.SetCursor(CursorType.Normal, Cursors.Default)

            'スプリット属性設定
            Me.sprM1MenuIchiran_Sheet1.AutoGenerateColumns = False
            Me.sprM2GokeiIchiran_Sheet1.AutoGenerateColumns = False

            ' データテーブルを作成します
            ' Ｍ１＿メニュー一覧
            Me.sprM1MenuIchiran_Sheet1.SelectionPolicy = Model.SelectionPolicy.MultiRange
            Me.sprM1MenuIchiran_Sheet1.DataSource = Nothing
            ' 2021/08/13 #4813 DataSourceは、Paging処理で設定
            'Me.sprM1MenuIchiran_Sheet1.DataSource = qc001F04FormDto.SprM1MenuIchiran
            Me.sprM1MenuIchiran_Sheet1.Columns(0).DataField = "M1No"
            Me.sprM1MenuIchiran_Sheet1.Columns(1).DataField = "M1MenuNo"
            Me.sprM1MenuIchiran_Sheet1.Columns(2).DataField = "M1MenuNm"
            Me.sprM1MenuIchiran_Sheet1.Columns(3).DataField = "M1Syubetu"
            Me.sprM1MenuIchiran_Sheet1.Columns(4).DataField = "M1KeiyakuTani"
            Me.sprM1MenuIchiran_Sheet1.Columns(5).DataField = "M1Seikyu"
            Me.sprM1MenuIchiran_Sheet1.Columns(6).DataField = "M1Futai"
            Me.sprM1MenuIchiran_Sheet1.Columns(7).DataField = "M1AddonHissu"
            Me.sprM1MenuIchiran_Sheet1.Columns(8).DataField = "M1AddonSuisho"
            Me.sprM1MenuIchiran_Sheet1.Columns(9).DataField = "M1ItakuKibo"
            Me.sprM1MenuIchiran_Sheet1.Columns(10).DataField = "M1Sryo"
            Me.sprM1MenuIchiran_Sheet1.Columns(11).DataField = "M1NengakuTeika"
            Me.sprM1MenuIchiran_Sheet1.Columns(12).DataField = "M1NengakuNebikiPar"
            Me.sprM1MenuIchiran_Sheet1.Columns(13).DataField = "M1NengakuBinTnk"
            Me.sprM1MenuIchiran_Sheet1.Columns(14).DataField = "M1NengakuHiyo"
            Me.sprM1MenuIchiran_Sheet1.Columns(15).DataField = "M1GetsugakuTeika"
            Me.sprM1MenuIchiran_Sheet1.Columns(16).DataField = "M1GetsugakuNebikiPar"
            Me.sprM1MenuIchiran_Sheet1.Columns(17).DataField = "M1GetsugakuBinTnk"
            Me.sprM1MenuIchiran_Sheet1.Columns(18).DataField = "M1GetsugakuHiyo"
            Me.sprM1MenuIchiran_Sheet1.Columns(19).DataField = "M1MusyoShokiHiyo"
            Me.sprM1MenuIchiran_Sheet1.Columns(20).DataField = "M1ShokiHiyo"
            Me.sprM1MenuIchiran_Sheet1.Columns(21).DataField = "M1MusyoZuijiHiyo"
            Me.sprM1MenuIchiran_Sheet1.Columns(22).DataField = "M1ZuijiHiyo"
            Me.sprM1MenuIchiran_Sheet1.Columns(23).DataField = "M1GnkKbnChooseNm"
            Me.sprM1MenuIchiran_Sheet1.Columns(24).DataField = "M1HyojunGnk"
            Me.sprM1MenuIchiran_Sheet1.Columns(25).DataField = "M1AtoArari"
            Me.sprM1MenuIchiran_Sheet1.Columns(26).DataField = "M1ArariPar"
            Me.sprM1MenuIchiran_Sheet1.Columns(27).DataField = "M1GetsugakuMusyoMoNumChoose"
            Me.sprM1MenuIchiran_Sheet1.Columns(28).DataField = "CmbM1SettisakiComboChoose"
            Me.sprM1MenuIchiran_Sheet1.Columns(29).DataField = "CmbM1GroupComboChoose"
            Me.sprM1MenuIchiran_Sheet1.Columns(30).DataField = "M1SubTtl"
            Me.sprM1MenuIchiran_Sheet1.Columns(31).DataField = "M1SubTtl2"

            ' データテーブルを作成します
            ' Ｍ２＿メニュー一覧
            Me.sprM2GokeiIchiran_Sheet1.DataSource = Nothing
            Me.sprM2GokeiIchiran_Sheet1.DataSource = qc001F04FormDto.SprM2GokeiIchiran
            Me.sprM2GokeiIchiran_Sheet1.Columns(0).DataField = "SprM2GokeiShbt"
            Me.sprM2GokeiIchiran_Sheet1.Columns(1).DataField = "SprM2GokeiShbt2"
            Me.sprM2GokeiIchiran_Sheet1.Columns(2).DataField = "LblM2GokeiranNengakuTeika"
            Me.sprM2GokeiIchiran_Sheet1.Columns(3).DataField = "LblM2GokeiranNengakuNebikigaku"
            Me.sprM2GokeiIchiran_Sheet1.Columns(4).DataField = "LblM2GokeiranNengakuHiyo"
            Me.sprM2GokeiIchiran_Sheet1.Columns(5).DataField = "LblM2GokeiranGetsugakuTeika"
            Me.sprM2GokeiIchiran_Sheet1.Columns(6).DataField = "LblM2GokeiranGetsugakuNebikigaku"
            Me.sprM2GokeiIchiran_Sheet1.Columns(7).DataField = "LblM2GokeiranGetsugakuHiyo"
            Me.sprM2GokeiIchiran_Sheet1.Columns(8).DataField = "LblM2GokeiranShokiHiyo"
            Me.sprM2GokeiIchiran_Sheet1.Columns(9).DataField = "LblM2GokeiranZuijiHiyo"
            Me.sprM2GokeiIchiran_Sheet1.Columns(10).DataField = "LblM2GokeiranHyojunGnk"
            Me.sprM2GokeiIchiran_Sheet1.Columns(11).DataField = "LblM2GokeiranArarigaku"
            Me.sprM2GokeiIchiran_Sheet1.Columns(12).DataField = "LblM2GokeiranArariPar"
            Me.sprM2GokeiIchiran_Sheet1.Columns(13).DataField = "LblM2GetsugakuKansangoranGetsugakuHiyo"
            Me.sprM2GokeiIchiran_Sheet1.Columns(14).DataField = "LblM2GetsugakuKansangoranHyojunGnk"
            Me.sprM2GokeiIchiran_Sheet1.Columns(15).DataField = "LblM2GetsugakuKansangoranArarigaku"
            Me.sprM2GokeiIchiran_Sheet1.Columns(16).DataField = "LblM2GetsugakuKansangoranArariPar"

            ' M1＿NO
            Me.sprM1MenuIchiran_Sheet1.Columns(0).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿メニュー番号
            Me.sprM1MenuIchiran_Sheet1.Columns(1).HorizontalAlignment = CellHorizontalAlignment.Left

            ' M1＿メニュー名称
            Me.sprM1MenuIchiran_Sheet1.Columns(2).HorizontalAlignment = CellHorizontalAlignment.Left

            ' M1＿種別
            Me.sprM1MenuIchiran_Sheet1.Columns(3).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿契約
            Me.sprM1MenuIchiran_Sheet1.Columns(4).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿請求
            Me.sprM1MenuIchiran_Sheet1.Columns(5).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿付帯
            Me.sprM1MenuIchiran_Sheet1.Columns(6).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿アドオン（必須）
            Me.sprM1MenuIchiran_Sheet1.Columns(7).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿アドオン（推奨）
            Me.sprM1MenuIchiran_Sheet1.Columns(8).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿委託希望
            Me.sprM1MenuIchiran_Sheet1.Columns(9).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿数量
            Me.sprM1MenuIchiran_Sheet1.Columns(10).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿年額定価
            Me.sprM1MenuIchiran_Sheet1.Columns(11).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿月額定価
            Me.sprM1MenuIchiran_Sheet1.Columns(15).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿年額売価単価
            Me.sprM1MenuIchiran_Sheet1.Columns(13).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿月額売価単価
            Me.sprM1MenuIchiran_Sheet1.Columns(17).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿年額値引％ラベル
            Me.sprM1MenuIchiran_Sheet1.Columns(12).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿月額値引％ラベル
            Me.sprM1MenuIchiran_Sheet1.Columns(16).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿年額費用
            Me.sprM1MenuIchiran_Sheet1.Columns(14).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿月額費用
            Me.sprM1MenuIchiran_Sheet1.Columns(18).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿初期費用無償
            Me.sprM1MenuIchiran_Sheet1.Columns(19).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿初期費用
            Me.sprM1MenuIchiran_Sheet1.Columns(20).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿随時費用無償
            Me.sprM1MenuIchiran_Sheet1.Columns(21).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿随時費用
            Me.sprM1MenuIchiran_Sheet1.Columns(22).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿原価区分
            Me.sprM1MenuIchiran_Sheet1.Columns(23).HorizontalAlignment = CellHorizontalAlignment.Left

            ' M1＿標準原価
            Me.sprM1MenuIchiran_Sheet1.Columns(24).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿後粗利
            Me.sprM1MenuIchiran_Sheet1.Columns(25).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿粗利％
            Me.sprM1MenuIchiran_Sheet1.Columns(26).HorizontalAlignment = CellHorizontalAlignment.Right

            ' M1＿月額無償
            Me.sprM1MenuIchiran_Sheet1.Columns(27).HorizontalAlignment = CellHorizontalAlignment.Center

            ' M1＿設置先
            Me.sprM1MenuIchiran_Sheet1.Columns(28).HorizontalAlignment = CellHorizontalAlignment.Left

            ' M1＿グループ
            Me.sprM1MenuIchiran_Sheet1.Columns(29).HorizontalAlignment = CellHorizontalAlignment.Left

            ' Ｍ１＿サブタイトルチェック
            Me.sprM1MenuIchiran_Sheet1.Columns(30).HorizontalAlignment = CellHorizontalAlignment.Left

            Me.sprM1MenuIchiran_Sheet1.Columns(31).HorizontalAlignment = CellHorizontalAlignment.Left

            ' Ｍ２＿合計種別
            Me.sprM2GokeiIchiran_Sheet1.Columns(0).HorizontalAlignment = CellHorizontalAlignment.Left

            ' Ｍ２＿合計種別2
            Me.sprM2GokeiIchiran_Sheet1.Columns(1).HorizontalAlignment = CellHorizontalAlignment.Left

            ' Ｍ２＿合計欄_年額定価
            Me.sprM2GokeiIchiran_Sheet1.Columns(2).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_月額定価
            Me.sprM2GokeiIchiran_Sheet1.Columns(5).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_初期費用
            Me.sprM2GokeiIchiran_Sheet1.Columns(8).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_随時費用

            ' Ｍ２＿合計欄_年額値引額
            Me.sprM2GokeiIchiran_Sheet1.Columns(3).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_月額値引額
            Me.sprM2GokeiIchiran_Sheet1.Columns(6).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_年額費用
            Me.sprM2GokeiIchiran_Sheet1.Columns(4).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_月額費用
            Me.sprM2GokeiIchiran_Sheet1.Columns(7).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_標準原価
            Me.sprM2GokeiIchiran_Sheet1.Columns(10).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_粗利額
            Me.sprM2GokeiIchiran_Sheet1.Columns(11).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿合計欄_粗利％
            Me.sprM2GokeiIchiran_Sheet1.Columns(12).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿月額換算後欄_月額費用
            Me.sprM2GokeiIchiran_Sheet1.Columns(13).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿月額換算後欄_標準原価
            Me.sprM2GokeiIchiran_Sheet1.Columns(14).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿月額換算後欄_粗利額
            Me.sprM2GokeiIchiran_Sheet1.Columns(15).HorizontalAlignment = CellHorizontalAlignment.Right

            ' Ｍ２＿月額換算後欄_粗利％
            Me.sprM2GokeiIchiran_Sheet1.Columns(16).HorizontalAlignment = CellHorizontalAlignment.Right

            Dim richTxt As CellType.RichTextCellType = New CellType.RichTextCellType
            Me.sprM2GokeiIchiran_Sheet1.Columns(0).CellType = richTxt

            'ST1_#8507 START
            Dim FlatScrollBarRenderer1 As FarPoint.Win.Spread.FlatScrollBarRenderer = New FarPoint.Win.Spread.FlatScrollBarRenderer()
            Dim FlatScrollBarRenderer2 As FarPoint.Win.Spread.FlatScrollBarRenderer = New FarPoint.Win.Spread.FlatScrollBarRenderer()
            Dim FlatScrollBarRenderer3 As FarPoint.Win.Spread.FlatScrollBarRenderer = New FarPoint.Win.Spread.FlatScrollBarRenderer()
            Dim FlatScrollBarRenderer4 As FarPoint.Win.Spread.FlatScrollBarRenderer = New FarPoint.Win.Spread.FlatScrollBarRenderer()
            FlatScrollBarRenderer1.ArrowColor = System.Drawing.Color.FromArgb(CType(CType(121, Byte), Integer), CType(CType(121, Byte), Integer), CType(CType(121, Byte), Integer))
            FlatScrollBarRenderer1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
            FlatScrollBarRenderer1.BorderActiveColor = System.Drawing.Color.FromArgb(CType(CType(171, Byte), Integer), CType(CType(171, Byte), Integer), CType(CType(171, Byte), Integer))
            FlatScrollBarRenderer1.TrackBarBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
            Me.sprM1MenuIchiran.HorizontalScrollBar.Renderer = FlatScrollBarRenderer1
            Me.sprM1MenuIchiran.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded
            FlatScrollBarRenderer2.ArrowColor = System.Drawing.Color.FromArgb(CType(CType(121, Byte), Integer), CType(CType(121, Byte), Integer), CType(CType(121, Byte), Integer))
            FlatScrollBarRenderer2.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
            FlatScrollBarRenderer2.BorderActiveColor = System.Drawing.Color.FromArgb(CType(CType(171, Byte), Integer), CType(CType(171, Byte), Integer), CType(CType(171, Byte), Integer))
            FlatScrollBarRenderer2.TrackBarBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
            Me.sprM1MenuIchiran.VerticalScrollBar.Renderer = FlatScrollBarRenderer2
            Me.sprM1MenuIchiran.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded

            FlatScrollBarRenderer3.ArrowColor = System.Drawing.Color.FromArgb(CType(CType(121, Byte), Integer), CType(CType(121, Byte), Integer), CType(CType(121, Byte), Integer))
            FlatScrollBarRenderer3.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
            FlatScrollBarRenderer3.BorderActiveColor = System.Drawing.Color.FromArgb(CType(CType(171, Byte), Integer), CType(CType(171, Byte), Integer), CType(CType(171, Byte), Integer))
            FlatScrollBarRenderer3.TrackBarBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
            Me.sprM2GokeiIchiran.HorizontalScrollBar.Renderer = FlatScrollBarRenderer3
            Me.sprM2GokeiIchiran.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded
            FlatScrollBarRenderer4.ArrowColor = System.Drawing.Color.FromArgb(CType(CType(121, Byte), Integer), CType(CType(121, Byte), Integer), CType(CType(121, Byte), Integer))
            FlatScrollBarRenderer4.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
            FlatScrollBarRenderer4.BorderActiveColor = System.Drawing.Color.FromArgb(CType(CType(171, Byte), Integer), CType(CType(171, Byte), Integer), CType(CType(171, Byte), Integer))
            FlatScrollBarRenderer4.TrackBarBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
            Me.sprM2GokeiIchiran.VerticalScrollBar.Renderer = FlatScrollBarRenderer4
            Me.sprM2GokeiIchiran.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded
            'ST1_#8507 END

            ClientLogUtil.Logger.DebugAP("QC001F04Form:InitSpread end")
        End Sub

        ''' <summary>
        ''' 幅記憶情報を画面へ反映
        ''' </summary>
        ''' <param name="meisaiHabaList"></param>
        Private Sub MeisaiHabaSetting(ByVal meisaiHabaList As List(Of SettingDto))
            ClientLogUtil.Logger.DebugAP("QC001F04Form:MeisaiHabaSetting start")
            Dim settingUtil As New SettingUtil("QC001F04", Nothing, Guid.NewGuid().ToString)

            For Each col As Column In Me.sprM1MenuIchiran_Sheet1.Columns
                If Not IsNothing(settingUtil.GetValueForSettingDto(meisaiHabaList, Consts.habakioku, col.Label, "NA")) AndAlso
                   CInt(settingUtil.GetValueForSettingDto(meisaiHabaList, Consts.habakioku, col.Label, "NA")) > 0 Then
                    col.Width = CInt(settingUtil.GetValueForSettingDto(meisaiHabaList, Consts.habakioku, col.Label, "NA"))
                End If
            Next
            ClientLogUtil.Logger.DebugAP("QC001F04Form:MeisaiHabaSetting end")
        End Sub

        ''' <summary>
        ''' 最初のページ
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub BtnFirstPage_Click(sender As Object, e As EventArgs) Handles btnSentouHyouji.Click

            '開始処理を行う
            InitProcess()

            Me.changedFlg = False

            txtGenzaiNoPage.Text = "1"
            Me.Paging()

            Me.changedFlg = True

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 前のページ
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub BtnPrePage_Click(sender As Object, e As EventArgs) Handles btnMaePegeHyouji.Click

            '開始処理を行う
            InitProcess()

            Me.changedFlg = False

            txtGenzaiNoPage.Text = (CInt(txtGenzaiNoPage.Text) - 1).ToString
            Me.Paging()

            Me.changedFlg = True

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 次のページ
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub BtnNextPage_Click(sender As Object, e As EventArgs) Handles btnTsugiPageHyouji.Click

            '開始処理を行う
            InitProcess()

            Me.changedFlg = False

            txtGenzaiNoPage.Text = (CInt(txtGenzaiNoPage.Text) + 1).ToString
            Me.Paging()

            Me.changedFlg = True

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 最後のページ
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub BtnLastPage_Click(sender As Object, e As EventArgs) Handles btnSaigouHyouji.Click

            '開始処理を行う
            InitProcess()

            Me.changedFlg = False

            txtGenzaiNoPage.Text = lblSouPageSuu.Text
            Me.Paging()

            Me.changedFlg = True

            '終了処理を実行する
            EndProcess()

        End Sub

        ''' <summary>
        ''' 現在のページ数変更
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub TxtCurrentPage_Leave(sender As Object, e As EventArgs) Handles txtGenzaiNoPage.Leave

            '#12150 2022.08.08 START
            Me.bFlag = False
            '#12150 2022.08.08 END

            If Not Me.changedFlg Then
                Exit Sub
            End If

            Me.changedFlg = False
            If Regex.IsMatch(txtGenzaiNoPage.Text, "^\d+$") Then
                '#12150 2022.08.08 START
                Me.bFlag = True
                '#12150 2022.08.08 END
                Me.Paging()
            Else
                txtGenzaiNoPage.Text = oldCurrentPage
            End If

            Me.changedFlg = True

        End Sub

        ''' <summary>
        ''' 1ページあたりの表示件数変更
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub TxtPerPageSize_Leave(sender As Object, e As EventArgs) Handles txtIchiPageNoKensuu.Leave

            If Not Me.changedFlg Then
                Exit Sub
            End If

            Me.changedFlg = False
            '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　1ページあたりの表示件数変更　Start
            '変更しない場合、処理終了
            'If Not String.Equals(txtPerPageSize.Text, oldPerPageSize) Then
            '    If Regex.IsMatch(txtPerPageSize.Text, "^\d+$") Then
            '        txtCurrentPage.Text = "1"
            '        Me.Paging()
            '    Else
            '        txtPerPageSize.Text = oldPerPageSize
            '    End If
            'End If
            '' #8196 No5 START
            qc001F04FormDto.PerPageSize = txtIchiPageNoKensuu.Text
            '' #8196 No5 END

            '#IT1_0936 2022.7.1 zeng 改修 Start
            '1頁の行数に数値が入力されている場合、設定値を保存する
            Dim settingUtilInit As New SettingUtil(FormId, FormId, Guid.NewGuid().ToString)
            Dim UpdatesettingDtoList As New List(Of SettingDto)
            '１頁表示件数をDBへ反映
            If Not String.IsNullOrWhiteSpace(txtIchiPageNoKensuu.Text) Then
                FormId = FORM_ID
                '設定値トランDTOリスト
                Dim settingInitDto = New SettingDto With {
                    .ItemDivision = "G",
                    .ItemId = FormId，
                    .Remarks = String.Empty,
                    .Section = "１頁の件数",
                    .SettingFileName = "NA"，
                    .TenkaCode = "ZZZZZ",
                    .UserId = ApplicationScope.LoginInfo.SyainCode,
                    .Param = "商品明細"
                }

                settingInitDto.Value = txtIchiPageNoKensuu.Text
                settingInitDto.UpdateFlag = True
                UpdatesettingDtoList.Add(settingInitDto)

                '(3)-3.共通部品呼び出し
                settingUtilInit.UpdateSettingData(UpdatesettingDtoList)
            End If
            '2下記の設定がないと、CtrlPage関数でDBからではなくて、QC001SettingDataから取ってきて設定してしまう
            Dim CommonBaseBusinessFormDto As BaseBusinessFormDto = SharedMemoryScope.InstanceData.GetValue("BaseBusinessFormDto")
            If IsNothing(CommonBaseBusinessFormDto) Then
                CommonBaseBusinessFormDto = New BaseBusinessFormDto
                CommonBaseBusinessFormDto.formId = FormId
                '(2)-3 BaseBusinessformDtoを共有メモリに格納する
                SharedMemoryScope.InstanceData.PutValue("BaseBusinessFormDto", CommonBaseBusinessFormDto)
            End If

            '#IT1_0936 2022.7.1 zeng 改修 End

            ' BUSINESSCOMFormのCtrlPageを呼び出す。
            Dim dtoList = CtrlPage(qc001F04FormDto.SprM1MenuIchiran, BusinessUtils.ConvertToInteger(Me.txtIchiPageNoKensuu.Text),
                    BusinessUtils.ConvertToInteger(Me.txtGenzaiNoPage.Text), BusinessUtils.ConvertToInteger(Me.lblSouPageSuu.Text)).Cast(Of QC001F04M1Dto)().ToList()

            ' BUSINESSCOMFormのCtrlUsableButtonを呼び出す。
            CtrlUsableButton(Me)

            '12150 2022.08.08 START
            Me.bFlag = True
            '12150 2022.08.08 END

            ' 戻り値（画面表示DTOリスト）をスプレッド一覧に設定する。
            'qc001F04FormDto.SprM1MenuIchiran = New BindingList < QC001F04M1Dto > (dtoList)
            Me.Paging()


            '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　1ページあたりの表示件数変更　End

            Me.changedFlg = True

        End Sub

        ''' <summary>
        ''' 改ページ
        ''' </summary>
        Private Sub Paging(Optional resetBackColor As Boolean = True)
            ClientLogUtil.Logger.DebugAP("QC001F04Form:Paging start")
            ' 最後空白行
            If (qc001F04FormDto.SprM1MenuIchiran.Count = 0 OrElse
               Not String.IsNullOrEmpty(qc001F04FormDto.SprM1MenuIchiran(qc001F04FormDto.SprM1MenuIchiran.Count - 1).M1MenuNo)) AndAlso
               Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Modify) AndAlso
               Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.DemoKirikae) AndAlso
               Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.DemoKasidasi) AndAlso
               Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Sinsei) AndAlso
               Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Print) AndAlso
               Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.TanNendoUpdate) AndAlso
               Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                ' 行挿入
                qc001F04FormDto.SprM1MenuIchiran.Insert(qc001F04FormDto.SprM1MenuIchiran.Count, New QC001F04M1Dto)
            End If
            ' spread初期化
            Me.InitSpread()
            qc001F04FormDto.SprM1MenuIchiranBk = CType(Copy(qc001F04FormDto.SprM1MenuIchiran), BindingList(Of QC001F04M1Dto))
            qc001F04FormDto.SprM2GokeiIchiranBk = CType(Copy(qc001F04FormDto.SprM2GokeiIchiran), BindingList(Of QC001F04M2Dto))


            ' 2021/08/12 #4813 STA
            'Dim totalSize As Integer = qc001F04FormDto.SprM1MenuIchiran.Count
            Dim totalSize As Integer = 0
            If (Me.cmbMeisaiHyojiSetteiSettisakiCombo.Text.Equals(Consts.zente)) Then
                totalSize = qc001F04FormDto.SprM1MenuIchiran.Count
            Else
                For Each tempMeisai In qc001F04FormDto.SprM1MenuIchiran
                    ' 設置先がComboBoxの選択値と一致またはNothing（最後の空行）
                    '#12311 2022.08.04 START
                    If (IsNothing(tempMeisai.CmbM1SettisakiComboChoose)) OrElse
                        (tempMeisai.CmbM1SettisakiComboCode.Equals(qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki)) Then
                        '#12311 2022.08.04 END
                        totalSize = totalSize + 1
                    End If
                Next
            End If
            ' 2021/08/12 #4813 END

            '#12312 2022/08/04 OEC)Fujiwara UPD start
            ''#12201 2022.08.03 START
            'For num = 0 To qc001F04FormDto.SprM1MenuIchiran.Count - 1
            '    If IsNothing(qc001F04FormDto.SprM1MenuIchiran(num).M1MenuNo) Then
            '        totalSize = totalSize - 1
            '    End If
            'Next
            ''#12201 2022.08.03 END
            Dim kensuCount As Integer = totalSize
            For num = 0 To qc001F04FormDto.SprM1MenuIchiran.Count - 1
                If IsNothing(qc001F04FormDto.SprM1MenuIchiran(num).M1MenuNo) Then
                    kensuCount = kensuCount - 1
                End If
            Next
            '#12312 2022/08/04 OEC)Fujiwara UPD end


            '2021/08/13 #4813
            Dim currentList = New List(Of QC001F04M1Dto)

            If totalSize > 0 Then
                Dim totalPage As Integer
                Dim currentPage As Integer
                Dim perPageSize As Integer

                '1ページあたりの表示件数が空白の場合
                If String.Empty.Equals(txtIchiPageNoKensuu.Text) OrElse CInt(txtIchiPageNoKensuu.Text) < 1 Then
                    'デフォルト値設定
                    ' 2021/08/12 #4813
                    'perPageSize = Consts.PerPageSize_QC001F04
                    perPageSize = qc001F04FormDto.PerPageSize
                Else
                    perPageSize = CInt(txtIchiPageNoKensuu.Text)
                End If

                If Math.Ceiling(totalSize / perPageSize) = 1 Then
                    totalPage = 1
                Else
                    totalPage = CInt(Math.Ceiling(totalSize / perPageSize))
                End If

                '現在のページが空白の場合
                If String.Empty.Equals(txtGenzaiNoPage.Text) OrElse CInt(txtGenzaiNoPage.Text) < 1 Then
                    currentPage = 1
                Else
                    currentPage = CInt(txtGenzaiNoPage.Text)
                End If

                '現在のページが最大ページ数より大きい場合
                If currentPage > totalPage Then
                    currentPage = totalPage
                End If

                '表示件数
                Dim dispSize As Integer = perPageSize
                If currentPage = totalPage Then
                    dispSize = totalSize - (totalPage - 1) * perPageSize
                End If

                'ボタン制御
                btnSentouHyouji.Enabled = True
                btnMaePegeHyouji.Enabled = True
                btnTsugiPageHyouji.Enabled = True
                btnSaigouHyouji.Enabled = True

                '1ページの場合
                If currentPage = 1 Then
                    btnSentouHyouji.Enabled = False
                    btnMaePegeHyouji.Enabled = False
                End If

                '最大ページの場合
                If currentPage = totalPage Then
                    btnTsugiPageHyouji.Enabled = False
                    btnSaigouHyouji.Enabled = False
                End If

                '#12312 2022/08/04 OEC)Fujiwara UPD start
                ''＃12201 2022.08.03 START
                ''lblGaitouKensuu.Text = CStr(totalSize - 1)
                'lblGaitouKensuu.Text = CStr(totalSize)
                ''＃12201 2022.08.03 END
                lblGaitouKensuu.Text = CStr(kensuCount)
                '#12312 2022/08/04 OEC)Fujiwara UPD end
                lblSouPageSuu.Text = CStr(totalPage)
                txtGenzaiNoPage.Text = CStr(currentPage)
                txtIchiPageNoKensuu.Text = CStr(perPageSize)

                '現在のページのデータ取得
                ' 2021/08/12 #4813 STA 2021/8/17追加修正
                'Dim currentList = qc001F04FormDto.SprM1MenuIchiran.ToList().GetRange((currentPage - 1) * perPageSize, dispSize)
                Dim dataSize As Integer = 0
                Dim sta As Integer = (currentPage - 1) * perPageSize
                For i As Integer = 0 To qc001F04FormDto.SprM1MenuIchiran.Count - 1
                    If (Me.cmbMeisaiHyojiSetteiSettisakiCombo.Text.Equals(Consts.zente)) Then
                        dataSize = dataSize + 1
                        If sta < dataSize And dataSize <= sta + perPageSize Then
                            currentList.Add(qc001F04FormDto.SprM1MenuIchiran(i))
                            If dataSize >= sta + perPageSize Then
                                Exit For
                            End If
                        End If
                        '#12311 2022.08.04 START
                    ElseIf (Not IsNothing(qc001F04FormDto.SprM1MenuIchiran(i).CmbM1SettisakiComboChoose)) _
                        AndAlso (qc001F04FormDto.SprM1MenuIchiran(i).CmbM1SettisakiComboCode.Equals(qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki)) Then
                        '#12311 2022.08.04 END
                        dataSize = dataSize + 1
                        If sta < dataSize And dataSize <= sta + perPageSize Then
                            currentList.Add(qc001F04FormDto.SprM1MenuIchiran(i))
                            If dataSize >= sta + perPageSize Then
                                Exit For
                            End If
                        End If
                    ElseIf IsNothing(qc001F04FormDto.SprM1MenuIchiran(i).CmbM1SettisakiComboChoose) _
                        AndAlso (i = qc001F04FormDto.SprM1MenuIchiran.Count - 1) Then
                        currentList.Add(qc001F04FormDto.SprM1MenuIchiran(i))
                        dataSize = dataSize + 1
                    End If
                Next
                ' 2021/08/12 #4813 END
                sprM1MenuIchiran_Sheet1.DataSource = currentList
                qc001F04FormDto.SprM1MenuIchiranBk2 = currentList
                Me.ControlGamenKomoku(currentList, resetBackColor)
            Else
                '一覧をクリアする
                ' 2021/08/13 #4813 START
                'sprM1MenuIchiran_Sheet1.DataSource = qc001F04FormDto.SprM1MenuIchiran
                'Me.ControlGamenKomoku(qc001F04FormDto.SprM1MenuIchiran.ToList, resetBackColor)
                If qc001F04FormDto.SprM1MenuIchiran.Count > 0 Then
                    currentList.Add(qc001F04FormDto.SprM1MenuIchiran(qc001F04FormDto.SprM1MenuIchiran.Count - 1))
                End If
                sprM1MenuIchiran_Sheet1.DataSource = currentList
                Me.ControlGamenKomoku(currentList, resetBackColor)
                ' 2021/08/13 #4813 END

                'ページ初期化
                '#12312 2022/08/04 OEC)Fujiwara UPD start
                'lblSouPageSuu.Text = String.Empty
                'lblGaitouKensuu.Text = String.Empty
                'txtIchiPageNoKensuu.Text = String.Empty
                'txtGenzaiNoPage.Text = String.Empty
                lblSouPageSuu.Text = "1"
                lblGaitouKensuu.Text = "0"
                txtIchiPageNoKensuu.Text = qc001F04FormDto.PerPageSize
                txtGenzaiNoPage.Text = "1"
                '#12312 2022/08/04 OEC)Fujiwara UPD end

                'ボタン制御
                btnSentouHyouji.Enabled = False
                btnMaePegeHyouji.Enabled = False
                btnTsugiPageHyouji.Enabled = False
                btnSaigouHyouji.Enabled = False
            End If

            oldPerPageSize = txtIchiPageNoKensuu.Text
            oldCurrentPage = txtGenzaiNoPage.Text
            ClientLogUtil.Logger.DebugAP("QC001F04Form:Paging end")
        End Sub

        ''' <summary>
        ''' 選択行の取得
        ''' </summary>
        Private Sub GetRowIndex()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:GetRowIndex start")
            Dim rowNoList = New List(Of Integer)
            ' 選択行の取得
            qc001F04FormDto.SelectedRowIndex = New List(Of Integer)
            '#6614
            'ST1_#4491
            '#6472 start
            'Dim Selections() As FarPoint.Win.Spread.Model.CellRange
            'Selections = sprM1MenuIchiran.ActiveSheet.GetSelections
            Dim rowIndex As Integer = -1
            Dim activeCellRanges() As FarPoint.Win.Spread.Model.CellRange = sprM1MenuIchiran.ActiveSheet.GetSelections()
            If activeCellRanges Is Nothing OrElse activeCellRanges.Length = 0 Then
                '#12932 2022/09/25 start
                If Not IsNothing(sprM1MenuIchiran_Sheet1) AndAlso
                    sprM1MenuIchiran_Sheet1.RowCount - 1 >= sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow Then
                    rowIndex = sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow
                Else
                    rowIndex = 0
                End If
                ' #13498 start
                If sprM1MenuIchiran_Sheet1.RowCount > 0 Then
                    rowNoList.Add(getRowNo(rowIndex))
                    qc001F04FormDto.SelectedRowIndex.AddRange(rowNoList)
                End If
                ' #13498 end
            Else
                For Each activeCellRange As FarPoint.Win.Spread.Model.CellRange In activeCellRanges
                    For i = 0 To activeCellRange.RowCount() - 1
                        rowIndex = activeCellRange.Row() + i
                        rowNoList.Add(getRowNo(rowIndex))
                        '#12263 2022.08.07 START
                        'qc001F04FormDto.SelectedRowIndex.AddRange(rowNoList)
                        '#12263 2022.08.07 END
                    Next
                Next
                '#12263 2022.08.07 START
                qc001F04FormDto.SelectedRowIndex.AddRange(rowNoList)
                '#12263 2022.08.07 END
            End If
            ' ST#5744 START
            'For Each activeCellRange As FarPoint.Win.Spread.Model.CellRange In Selections
            '    For i = 0 To activeCellRange.RowCount() - 1
            '        '8259 Start
            '        Dim rowNo As Integer = getRowNo(activeCellRange.Row)
            '        '8259 End
            '        If Not qc001F04FormDto.SelectedRowIndex.Contains(rowNo) Then
            '            If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnSyoriMode, Consts.SyoriMode.Change) Then
            '                If Not String.Equals(qc001F04FormDto.SprM1MenuIchiran(rowNo).Status, "3") Then
            '                    qc001F04FormDto.SelectedRowIndex.Add(rowNo)
            '                End If
            '            Else
            '                qc001F04FormDto.SelectedRowIndex.Add(rowNo)
            '            End If
            '        End If
            '    Next
            'Next
            ' ST#5744 END
            '#6472 end
            ClientLogUtil.Logger.DebugAP("QC001F04Form:GetRowIndex end")
        End Sub

        '8208 Start
        ''' <summary>
        ''' 選択親子メニュー取得
        ''' </summary>
        Public Function GetOyakoMenuChoose() As Boolean
            ClientLogUtil.Logger.DebugAP("QC001F04Form:GetOyakoMenuChoose start")

            For Each i In qc001F04FormDto.SelectedRowIndex
                If String.Equals(qc001F04FormDto.SprM1MenuIchiran(i).M1Syubetu, Consts.M1Syubetu.ka) AndAlso
                    Not String.Equals(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(i).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), "0") AndAlso
                    String.IsNullOrEmpty(CStr(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(i).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"))) Then
                    MessageDialogUtil.ShowError(MessageUtil.GetDialogProperty(BusinessMessageConst.EK1485))
                    Return False
                End If
            Next
            Dim currentPage As Integer = CInt(txtGenzaiNoPage.Text)
            Dim pageSize As Integer = CInt(txtIchiPageNoKensuu.Text)
            Dim SelectedRowIndex1 As New List(Of Integer)
            For Each i In qc001F04FormDto.SelectedRowIndex
                If Not String.IsNullOrEmpty(qc001F04FormDto.SprM1MenuIchiran(i).SerMenuno) AndAlso
                        String.Equals(qc001F04FormDto.SprM1MenuIchiran(i).SerMenuno, qc001F04FormDto.SprM1MenuIchiran(i).M1NaibuNo) AndAlso
                         String.Equals(qc001F04FormDto.SprM1MenuIchiran(i).M1MenuSeq, qc001F04FormDto.SprM1MenuIchiran(i).M1MenuSeq) Then
                    For Each m1Row In qc001F04FormDto.SprM1MenuIchiran
                        If String.Equals(qc001F04FormDto.SprM1MenuIchiran(i).SerMenuno, m1Row.SerMenuno) AndAlso
                            String.Equals(qc001F04FormDto.SprM1MenuIchiran(i).CmbM1SettisakiComboChoose, m1Row.CmbM1SettisakiComboChoose) AndAlso
                             String.Equals(qc001F04FormDto.SprM1MenuIchiran(i).M1MenuSeq, m1Row.M1MenuSeq) Then
                            Dim selRownum As Integer
                            selRownum = CInt(m1Row.M1No) - 1
                            Dim rowNo As Integer = selRownum + pageSize * (currentPage - 1)
                            If Not qc001F04FormDto.SelectedRowIndex.Contains(rowNo) Then
                                If String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnSyoriMode, Consts.SyoriMode.Change) Then
                                    If Not String.Equals(qc001F04FormDto.SprM1MenuIchiran(rowNo).Status, "3") Then
                                        SelectedRowIndex1.Add(rowNo)
                                    End If
                                Else
                                    SelectedRowIndex1.Add(rowNo)
                                End If
                            End If
                        End If
                    Next
                End If
            Next
            If SelectedRowIndex1.Count > 0 Then
                qc001F04FormDto.SelectedRowIndex.AddRange(SelectedRowIndex1)
            End If

            ClientLogUtil.Logger.DebugAP("QC001F04Form:GetOyakoMenuChoose end")
            Return True

        End Function
        '8208 End

        ''' <summary>
        ''' オプションメニューの単一行選択は不可チェック
        ''' </summary>
        Public Function CheckMenuChoose(ByVal formdto As QC001F04FormDto, ByVal num As Integer) As Boolean
            ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose start")
            '2022.07.22 #11882 ADD-START インデックスオーバー防止対応 異常値で参照しないように修正
            If formdto.SprM1MenuIchiran.Count > num Then
                '2022.07.22 #11882 ADD-END インデックスオーバー防止対応 異常値で参照しないように修正
                If String.Equals(formdto.SprM1MenuIchiran(num).M1Syubetu, Consts.M1Syubetu.ka) Then
                    '仮想サービスメニュー番号が設定されている場合は、オプションメニューでも単一行選択可
                    If String.Equals(GetValueDic(formdto.SprM1MenuIchiran(num).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) Then
                        If Not String.IsNullOrEmpty(CType(GetValueDic(formdto.SprM1MenuIchiran(num).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"), String)) Then
                            ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose Return1")
                            Return True
                        End If
                    End If

                    '共有_たよトラン情報
                    Dim qc001Mjta = SharedComClient.InstanceData.QC001_MJTA
                    Dim qc001MjtakeiDto = qc001Mjta.QC001_MJTAKEIDTOList.Find(Function(v) String.Equals(v.MJTAKEIDTO.KEY_MJ, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_MJ) AndAlso
                                                                                      String.Equals(v.MJTAKEIDTO.KEY_MJ_HAN, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_MJ_HAN) AndAlso
                                                                                      String.Equals(v.MJTAKEIDTO.KEY_GEN, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_GEN) AndAlso
                                                                                      String.Equals(v.MJTAKEIDTO.KEY_EDA, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_EDA))
                    If Not IsNothing(qc001MjtakeiDto) Then
                        Dim qc001MjtamnuDto = qc001MjtakeiDto.QC001_MJTAMNUDTOList.Find(Function(v) String.Equals(v.MJTAMNUDTO.KEY_MJ, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_MJ) AndAlso
                                                                                      String.Equals(v.MJTAMNUDTO.KEY_MJ_HAN, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_MJ_HAN) AndAlso
                                                                                      String.Equals(v.MJTAMNUDTO.KEY_GEN, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_GEN) AndAlso
                                                                                      String.Equals(v.MJTAMNUDTO.KEY_EDA, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_EDA) AndAlso
                                                                                      String.Equals(v.MJTAMNUDTO.KEY_CNT, formdto.SprM1MenuIchiran(num).HENKOMAE_KEY_CNT))

                        '2022.07.22 #11882 ADD-START NullException防止対応
                        If Not IsNothing(qc001MjtamnuDto) Then
                            '2022.07.22 #11882 ADD-END NullException防止対応
                            If String.IsNullOrEmpty(qc001MjtamnuDto.MJTAMNUDTO.SER_MENUNO) Then
                                ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose Return2")
                                Return True
                            Else
                                '#12257 2022/08/29 ST MOD 並び替え機能の仕様変更対応
                                If String.Equals(qc001MjtamnuDto.MJTAMNUDTO.SER_MENUNO, formdto.SprM1MenuIchiran(num).M1NaibuNo) AndAlso
                                    String.Equals(GetValueDic(formdto.SprM1MenuIchiran(num).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU0) Then
                                    'サービスメニューの場合
                                    ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose Return3")
                                    Return True
                                ElseIf String.Equals(GetValueDic(formdto.SprM1MenuIchiran(num).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU1) Then
                                    'オプションメニューの場合
                                    ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose Return4")
                                    Return False
                                ElseIf String.Equals(GetValueDic(formdto.SprM1MenuIchiran(num).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), Consts.OPTIONSYUBETU.OPTIONSYUBETU2) Then
                                    'サブオプションメニューの場合
                                    ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose Return5")
                                    Return False
                                End If
                                '    For Each i As Integer In formdto.SelectedRowIndex
                                '        If String.Equals(qc001MjtamnuDto.MJTAMNUDTO.SER_MENUNO, formdto.SprM1MenuIchiran(i).M1NaibuNo) Then
                                '            ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose Return3")
                                '            Return True
                                '        End If
                                '    Next
                                '    ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose Return4")
                                '    Return False
                                '#12257 2022/08/29 ED MOD 並び替え機能の仕様変更対応
                            End If
                            '2022.07.22 #11882 ADD-START NullException防止対応
                        End If
                        '2022.07.22 #11882 ADD-END NullException防止対応
                    End If
                    '2022.07.22 #11882 ADD-START インデックスオーバー防止対応 異常値で参照しないように修正
                End If
                '2022.07.22 #11882 ADD-END インデックスオーバー防止対応 異常値で参照しないように修正
            End If
            ClientLogUtil.Logger.DebugAP("QC001F04Form:CheckMenuChoose end")
            Return True

        End Function

        ''' <summary>
        ''' フォームからＤＴＯへ編集
        ''' </summary>
        Private Sub BindFormToDto()
            qc001F04FormDto.SprHoshuRyokinSansyutsuKijunDate = String.Format("{0:yyyy/MM/dd}", Me.sprHoshuRyokinSansyutsuKijunDate.Value)
            qc001F04FormDto.CmbMeisaiHyojiSetteiSettisaki = CType(Me.cmbMeisaiHyojiSetteiSettisakiCombo.SelectedValue, String)
            qc001F04FormDto.CmbHoshuKbn = CType(Me.cmbHoshuKbnCombo.SelectedValue, String)
            qc001F04FormDto.SprHoshuRyokinSansyutsuKijunDate = String.Format("{0:yyyy/MM/dd}", Me.sprHoshuRyokinSansyutsuKijunDate.Value)
            qc001F04FormDto.TxtKeiyakuShikibetsu2 = Me.txtKeiyakuShikibetsu2.Text
            qc001F04FormDto.TxtHoshuryoCmt = Me.txtHoshuryoCmt.Text
            If Me.rdoNebikiSetteiMenuBetsuRadio.Checked Then
                qc001F04FormDto.RdoNebikiSetteiMenuBetsuRadio = Consts.menyuu
                qc001F04FormDto.RdoNebikiSetteiZidoAnbunRadio = Consts.menyuu
            Else
                qc001F04FormDto.RdoNebikiSetteiMenuBetsuRadio = Consts.jidoubunsuu
                qc001F04FormDto.RdoNebikiSetteiZidoAnbunRadio = Consts.jidoubunsuu
            End If
            qc001F04FormDto.CmbNebikiSetteiMarumeSettei = CType(Me.cmbNebikiSetteiMarumeSetteiCombo.SelectedValue, String)
            If Me.rdoGokeiHyojiSetteiHoshuRadio.Checked Then
                qc001F04FormDto.RdoGokeiHyojiSetteiHoshuRadio = Consts.hoxyu
                qc001F04FormDto.RdoGokeiHyojiSetteiKakinRadio = Consts.hoxyu
            Else
                qc001F04FormDto.RdoGokeiHyojiSetteiHoshuRadio = Consts.kakin
                qc001F04FormDto.RdoGokeiHyojiSetteiKakinRadio = Consts.kakin
            End If
        End Sub

        ''' <summary>
        ''' Dictionaryから値を取得する
        ''' </summary>
        ''' <param name="dic"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Private Function GetValueDic(ByVal dic As Dictionary(Of String, Object), ByVal key As String) As Object
            If Not IsNothing(dic) Then
                If dic.ContainsKey(key) Then
                    Return dic.Item(key)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 選択行変更の場合
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ' ### UPD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
        'Private Sub sprM1MenuIchiran_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles sprM1MenuIchiran.SelectionChanged
        Private Sub sprM1MenuIchiran_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            ' ### UPD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

            '#4401 START
            '参照モード以外の場合
            If Not String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then
                '画面＿並び替え▲ボタン
                If qc001F04FormDto.SprM1MenuIchiran.Count > 1 Then
                    ' 選択行の行番号取得
                    '#6144
                    Dim selectRowIndex = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)

                    If selectRowIndex = 0 Then
                        btnNarabikaeUp.Enabled = False
                    Else
                        btnNarabikaeUp.Enabled = True
                    End If
                Else
                    btnNarabikaeUp.Enabled = False
                End If

                '画面＿並び替え▼ボタン
                If qc001F04FormDto.SprM1MenuIchiran.Count > 1 Then
                    '#6144
                    If sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow >= 0 Then
                        ' 選択行の行番号取得
                        '#6144
                        Dim selectRowIndex = GetIndex(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow)

                        ' 選択行の行数を取得する
                        '#6144
                        Dim selectRowCnt = sprM1MenuIchiran_Sheet1.Models.Selection.LeadRow - sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow + 1

                        '最後の行を選択する場合
                        '#4902
                        If selectRowIndex + selectRowCnt = qc001F04FormDto.SprM1MenuIchiran.Count - 1 Then
                            btnNarabikaeDown.Enabled = False
                        Else
                            btnNarabikaeDown.Enabled = True
                        End If
                    End If
                Else
                    btnNarabikaeDown.Enabled = False
                End If
            End If

            If Not (Me.sprM1ShiftFlg OrElse Me.sprM1CtrlFlg) Then
                'ShiftやCtrl押下中でなければ、行選択状態をクリア
                allSelectRowList.Clear()
                sprM1MenuIchiran.ActiveSheet.ClearSelection()
            End If

            '「No」列が選択された場合
            If (e.Range.Column = 0 OrElse e.Range.Column = -1 OrElse 0 < allSelectRowList.Count) OrElse
                String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Reference) Then

                '行選択
                If Me.sprM1ShiftFlg Then
                    'Shift押下時は選択範囲を選択行に追加
                    For count = 0 To e.Range.RowCount - 1
                        Dim rowIndex = e.Range.Row + count
                        If Not allSelectRowList.Contains(rowIndex) Then
                            allSelectRowList.Add(rowIndex)
                        End If
                    Next
                ElseIf Me.sprM1CtrlFlg Then
                    'Ctrl押下時は選択中かどうかで、選択状態を切替
                    Dim rowIndex = e.Range.Row
                    If allSelectRowList.Contains(rowIndex) Then
                        allSelectRowList.Remove(rowIndex)
                    Else
                        allSelectRowList.Add(rowIndex)
                    End If
                Else
                    ' ADD #6848 START
                    ' 選択された範囲内の行番号を選択行に追加する
                    Dim DropStartLine As Integer = sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow
                    Dim DropEndLine As Integer = sprM1MenuIchiran_Sheet1.Models.Selection.LeadRow
                    'MODIFY 6848 下から上にドラッグする時 START
                    If DropStartLine > DropEndLine Then
                        Dim SwapTemp As Integer = DropStartLine
                        DropStartLine = DropEndLine
                        DropEndLine = SwapTemp
                    End If
                    'MODIFY 6848 END
                    For index = DropStartLine To DropEndLine
                        allSelectRowList.Add(index)
                    Next
                    ' ADD #6848 END
                    allSelectRowList.Add(e.Range.Row)
                End If

                '選択行を最新化
                sprM1MenuIchiran.ActiveSheet.ClearSelection()
                For Each rowIndex In allSelectRowList
                    sprM1MenuIchiran.ActiveSheet.AddSelection(rowIndex, -1, 1, -1)
                    CommUtility.SpreadAddSelectRow(sprM1MenuIchiran, rowIndex)
                Next
            End If

        End Sub


        '#4491メソッド2つ追加
        ''' <summary>
        ''' スプレッドコンボボックス選択時
        ''' </summary>
        Private Sub sprM1MenuIchiran_ComboDropDown(sender As Object, e As EditorNotifyEventArgs) Handles sprM1MenuIchiran.ComboDropDown

            '対象セルを選択状態として設定する。
            sprM1MenuIchiran.ActiveSheet.AddSelection(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow, sprM1MenuIchiran_Sheet1.Models.Selection.AnchorColumn, 1, 1)

        End Sub

        ''' <summary>
        ''' スプレッドセル選択時
        ''' </summary>
        Private Sub sprM1MenuIchiran_ButtonClicked(sender As Object, e As CellClickEventArgs) Handles sprM1MenuIchiran.CellClick

            '2022-11-13 ADD START #13370
            If e.Column = 0 Then
                qc001F04FormDto.EntireRowSelected = True
            End If
            '2022-11-13 ADD END #13370

            '対象セルを選択状態として設定する。
            sprM1MenuIchiran.ActiveSheet.AddSelection(sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow, sprM1MenuIchiran_Sheet1.Models.Selection.AnchorColumn, 1, 1)
            '#11822 Start
            'sprM1MenuIchiranの内容が変わるとして、sprM1MenuIchiranChangeFlgに設定する
            qc001F04FormDto.SprM1MenuIchiranChangeFlg = True
            '#11822 End

        End Sub
        ''' <summary>
        ''' 内容コピー
        ''' </summary>
        ''' <param name="copyFrom"></param>
        ''' <returns></returns>
        Private Function Copy(ByVal copyFrom As Object) As Object
            'メモリストリーム、バイナリフォーマッタを宣言する。
            ClientLogUtil.Logger.DebugAP("QC001F04Form:Copy start")
            Using memoryStream As New System.IO.MemoryStream()
                Dim binaryFormatter As New BinaryFormatter

                binaryFormatter.Serialize(memoryStream, copyFrom)
                memoryStream.Seek(0, SeekOrigin.Begin)

                'デシリアライズを行いディクショナリへ変換する
                ClientLogUtil.Logger.DebugAP("QC001F04Form:Copy end")
                Return binaryFormatter.Deserialize(memoryStream)
            End Using
        End Function

        ''' <summary>
        ''' QC001F00Formのアドオンチェックボタンに制御
        ''' </summary>
        '#12691 2022/08/23 START
        Private Sub CallQC001F00FormAddonCheckSeigyo(Optional checkStatus As String = QC001F00Form.STATUS_3)
            'Private Sub CallQC001F00FormAddonCheckSeigyo()
            '#12691 2022/08/23 END
            ClientLogUtil.Logger.DebugAP("QC001F04Form:CallQC001F00FormAddonCheckSeigyo start")
            ' フォームＤＴＯを退避
            SharedComClient.InstanceData.QC001F04FormDTO = qc001F04FormDto
            ' 親画面のイベントを呼び出す
            Dim QC001F00Form = CType(Me.Parent.Parent, QC001F00Form)
            '#12691 2022/08/23 START
            QC001F00Form.status = checkStatus
            '#12691 2022/08/23 END
            QC001F00Form.AddonCheckSeigyo()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:CallQC001F00FormAddonCheckSeigyo end")
        End Sub


        ''' <summary>
        ''' カーソルを手にする
        ''' </summary>
        Private Sub SplitContainer1_MouseMove(sender As Object, e As MouseEventArgs) Handles SplitContainer1.MouseMove

            SplitContainer1.Cursor = Cursors.Hand

        End Sub



        ''' <summary>
        ''' カーソルをデフォルトにする
        ''' </summary>
        Private Sub SplitContainer1_MouseLeave(sender As Object, e As EventArgs) Handles SplitContainer1.MouseLeave

            SplitContainer1.Cursor = Cursors.Default

        End Sub

        ' #6545 start
        ''' <summary>
        ''' 親メニュー行取得
        ''' </summary>
        Private Sub GetoyaRow(ByRef selectRowIndex As Integer, ByRef selectRowEnd As Integer, ByRef selectRowCnt As Integer, Optional syubetu As String = "")
            Dim selectRowIndexSubUse As Integer = selectRowIndex
            If String.Equals(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).M1Syubetu, Consts.M1Syubetu.ka) Then
                If String.Equals(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), "0") Then
                    Dim SerMenuno As String = qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).M1NaibuNo
                    '2022.09.27 ADD-START #13679 同一メニュー、同一設置先の場合を考慮
                    Dim Settisaki As String = qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).CmbM1SettisakiComboCode
                    '2022.09.27 ADD-END #13679 同一メニュー、同一設置先の場合を考慮
                    For num = selectRowIndex + 1 To qc001F04FormDto.SprM1MenuIchiran.Count - 1

                        '#12257 2022/08/29 ST MOD 並び替え機能の仕様変更対応
#Region "旧ソース"
                        ''#12202 2022.08.03 START
                        ''If String.Equals(SerMenuno, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                        ''    selectRowEnd += 1
                        ''Else
                        ''    Exit For
                        ''End If
                        'If String.Equals(SerMenuno, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                        '    selectRowEnd += 1
                        '    Exit For
                        'Else
                        '    If IsNothing(qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                        '        selectRowEnd += 1
                        '    End If
                        'End If
                        ''#12202 2022.08.03 END
#End Region
                        '2022.09.27 MOD-START #13679 同一メニュー、同一設置先の場合を考慮
                        'If String.Equals(SerMenuno, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                        '    'サービスメニューに紐づくオプションメニューのレコード行数分カウント
                        '    selectRowEnd += 1
                        'Else
                        '    If IsNothing(qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                        '        If num = qc001F04FormDto.SprM1MenuIchiran.Count - 1 Then
                        '            '最終行はカウントせず、処理終了
                        '            Exit For
                        '        Else
                        '            selectRowEnd += 1
                        '        End If
                        '    End If
                        'End If
                        '#12257 2022/08/29 ED MOD 並び替え機能の仕様変更対応
                        If String.Equals(syubetu, "Delete") Then
                            '行削除の場合
                            If String.Equals(SerMenuno, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) AndAlso String.Equals(Settisaki, qc001F04FormDto.SprM1MenuIchiran(num).CmbM1SettisakiComboCode) Then
                                'サービスメニューに紐づくオプションメニューのレコード行数分カウント
                                selectRowEnd = CInt(qc001F04FormDto.SprM1MenuIchiran(num).M1No)
                            Else
                                If IsNothing(qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                                    If num = qc001F04FormDto.SprM1MenuIchiran.Count - 1 Then
                                        '最終行はカウントせず、処理終了
                                        Exit For
                                    Else
                                        selectRowEnd += 1
                                    End If
                                End If
                            End If
                        Else
                            '行削除以外の場合
                            If String.Equals(SerMenuno, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                                'サービスメニューに紐づくオプションメニューのレコード行数分カウント
                                selectRowEnd += 1
                            Else
                                If IsNothing(qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                                    If num = qc001F04FormDto.SprM1MenuIchiran.Count - 1 Then
                                        '最終行はカウントせず、処理終了
                                        Exit For
                                    Else
                                        selectRowEnd += 1
                                    End If
                                End If
                            End If

                        End If
                        '2022.09.27 MOD-END #13679 同一メニュー、同一設置先の場合を考慮
                    Next

                    selectRowCnt = selectRowEnd - selectRowIndex

                ElseIf String.Equals(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), "1") AndAlso
                       Not String.IsNullOrEmpty(CStr(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).QC001S04MstDetailKakinDto, "VIRTUAL_SRV_MENUNO"))) Then
                    Dim SerMenuno As String = qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).M1NaibuNo
                    For num = selectRowIndex + 1 To qc001F04FormDto.SprM1MenuIchiran.Count - 1
                        If String.Equals(SerMenuno, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                            selectRowEnd += 1
                        Else
                            Exit For
                        End If
                    Next

                    selectRowCnt = selectRowEnd - selectRowIndex

                ElseIf String.Equals(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), "1") OrElse
                       String.Equals(Me.GetValueDic(qc001F04FormDto.SprM1MenuIchiran(selectRowIndex).QC001S04MstDetailKakinDto, "OPTION_SYUBETU"), "2") Then
                    Dim oyaMenunoDto = qc001F04FormDto.SprM1MenuIchiran.ToList.Find(Function(o) String.Equals(o.M1NaibuNo, qc001F04FormDto.SprM1MenuIchiran(selectRowIndexSubUse).SerMenuno) AndAlso
                                                                                                String.Equals(o.CmbM1SettisakiComboCode, qc001F04FormDto.SprM1MenuIchiran(selectRowIndexSubUse).CmbM1SettisakiComboCode))
                    If IsNothing(oyaMenunoDto) Then
                        Me.changedFlg = True
                        Exit Sub
                    End If

                    selectRowIndex = CInt(oyaMenunoDto.M1No) - 1
                    selectRowIndexSubUse = CInt(oyaMenunoDto.M1No) - 1
                    selectRowEnd = CInt(oyaMenunoDto.M1No)

                    For num = selectRowIndex + 1 To qc001F04FormDto.SprM1MenuIchiran.Count - 1
                        '#12202 2022.08.03 START
                        'If String.Equals(oyaMenunoDto.M1NaibuNo, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                        '    selectRowEnd += 1
                        'Else
                        '    Exit For
                        'End If
                        '2022.09.27 MOD-START #13679 先頭から2行だけしか削除されない不具合を修正
                        'If String.Equals(oyaMenunoDto.M1NaibuNo, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                        '    selectRowEnd += 1
                        '    Exit For
                        'Else
                        '    If IsNothing(qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                        '        selectRowEnd += 1
                        '    End If
                        'End If
                        ''#12202 2022.08.03 END
                        If String.Equals(syubetu, "Delete") Then
                            '行削除の場合
                            If String.Equals(oyaMenunoDto.M1NaibuNo, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) AndAlso String.Equals(oyaMenunoDto.CmbM1SettisakiComboCode, qc001F04FormDto.SprM1MenuIchiran(num).CmbM1SettisakiComboCode) Then
                                '親メニューが一致した場合、一番最後の行をセットする
                                selectRowEnd = CInt(qc001F04FormDto.SprM1MenuIchiran(num).M1No)
                            End If
                        Else
                            '行削除以外の場合
                            If String.Equals(oyaMenunoDto.M1NaibuNo, qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                                selectRowEnd += 1
                                Exit For
                            Else
                                If IsNothing(qc001F04FormDto.SprM1MenuIchiran(num).SerMenuno) Then
                                    selectRowEnd += 1
                                End If
                            End If
                        End If
                        '2022.09.27 MOD-END #13679 先頭から2行だけしか削除されない不具合を修正
                    Next

                    selectRowCnt = selectRowEnd - selectRowIndex
                End If
            Else
                selectRowCnt = 1
            End If
        End Sub
        ' #6545 end

        ''' <summary>
        ''' 内部明細の計算
        ''' </summary>
        Public Sub NaibuCalculateMesai(ByVal Columnnum As Integer, ByVal rownum As Integer)

            ' Ｍ１＿年額定価ロストフォーカス
            If Columnnum.Equals(11) Then
                If String.Equals(qc001F04FormDto.SprM1MenuIchiran(rownum).M1Syubetu, Consts.M1Syubetu.ho) Then
                    ' Ｍ１＿内部＿保守＿定価年額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuTeikaNengaku = qc001F04FormDto.SprM1MenuIchiran(rownum).M1NengakuTeika
                End If
            End If

            ' Ｍ１＿年額売価単価ロストフォーカス
            If Columnnum.Equals(13) Then
                If String.Equals(qc001F04FormDto.SprM1MenuIchiran(rownum).M1Syubetu, Consts.M1Syubetu.ho) Then
                    ' Ｍ１＿内部＿保守＿費用年額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuHiyoNengaku = qc001F04FormDto.SprM1MenuIchiran(rownum).M1NengakuBinTnk
                    ' Ｍ１＿内部＿保守＿値引年額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuNebikiNengaku = CType(CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuTeikaNengaku) - CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuHiyoNengaku), String)
                Else
                    ' Ｍ１＿内部＿課金＿値引単価年額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinNebikiTnkNengaku = CType(CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinTeikaTnkNengaku) -
                                                                                  CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1NengakuBinTnk), String)
                    ' Ｍ１＿内部＿課金＿値引年額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinNebikiNengaku = CType(CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinNebikiTnkNengaku) *
                                                              CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinKeisanTaisyoSryo), String)
                    ' Ｍ１＿内部＿課金＿費用年額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinHiyoNengaku = CType((CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinTeikaTnkNengaku) - CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinNebikiTnkNengaku)) *
                                                              CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinKeisanTaisyoSryo), String)
                End If
            End If

            ' Ｍ１＿月額売価単価ロストフォーカス
            If Columnnum.Equals(17) Then
                If String.Equals(qc001F04FormDto.SprM1MenuIchiran(rownum).M1Syubetu, Consts.M1Syubetu.ho) Then
                    ' Ｍ１＿内部＿保守＿費用年額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuHiyoNengaku = CType(CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1GetsugakuBinTnk) * 12, String)
                    ' Ｍ１＿内部＿保守＿値引年額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuNebikiNengaku = CType(CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1NengakuTeika) - CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuHiyoNengaku), String)
                Else
                    ' Ｍ１＿内部＿課金＿値引単価月額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinNebikiTnkGetsugaku = CType(CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinTeikaTnkGetsugaku) - CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1GetsugakuBinTnk), String)
                    ' Ｍ１＿内部＿課金＿値引月額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinNebikiGetsugaku = CType(CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinNebikiTnkGetsugaku) *
                                                              CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinKeisanTaisyoSryo), String)
                    ' Ｍ１＿内部＿課金＿費用月額
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinHiyoGetsugaku = CType((CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinTeikaTnkGetsugaku) - CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinNebikiTnkGetsugaku)) *
                                                              CLng(qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinKeisanTaisyoSryo), String)
                End If
            End If

            ' Ｍ１＿標準原価ロストフォーカス
            If Columnnum.Equals(24) Then
                If String.Equals(qc001F04FormDto.SprM1MenuIchiran(rownum).M1Syubetu, Consts.M1Syubetu.ho) Then
                    ' Ｍ１＿内部＿保守＿標準原価
                    qc001F04FormDto.SprM1MenuIchiran(rownum).M1HoshuHyojunGnk = qc001F04FormDto.SprM1MenuIchiran(rownum).M1HyojunGnk
                Else
                    ' ST1_#4801 START
                    If String.Equals(Trim(qc001F04FormDto.SprM1MenuIchiran(rownum).M1GnkKbnChoose), "Z") OrElse String.Equals(qc001F04FormDto.SprM1MenuIchiran(rownum).KOBETUFLG, Consts.ari) Then
                        If String.Equals(qc001F04FormDto.SprM1MenuIchiran(rownum).M1Seikyu, Consts.nen) Then
                            ' Ｍ１＿内部＿課金＿標準原価年額
                            qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinHyojunGnkNengaku = qc001F04FormDto.SprM1MenuIchiran(rownum).M1HyojunGnk
                        Else
                            ' Ｍ１＿内部＿課金＿標準原価月額
                            qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinHyojunGnkGetsugaku = qc001F04FormDto.SprM1MenuIchiran(rownum).M1HyojunGnk
                        End If
                    Else
                        If String.Equals(qc001F04FormDto.SprM1MenuIchiran(rownum).M1Seikyu, Consts.nen) Then
                            ' Ｍ１＿内部＿課金＿標準原価年額
                            qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinHyojunGnkMasutaNengaku = qc001F04FormDto.SprM1MenuIchiran(rownum).M1HyojunGnk
                        Else
                            ' Ｍ１＿内部＿課金＿標準原価月額
                            qc001F04FormDto.SprM1MenuIchiran(rownum).M1KakinHyojunGnkMasutaGetsugaku = qc001F04FormDto.SprM1MenuIchiran(rownum).M1HyojunGnk
                        End If
                    End If
                    ' ST1_#4801 END
                End If
            End If
        End Sub

        ''' <summary>
        ''' ページ遷移を考慮したインデックスを取得する
        ''' </summary>
        ''' <param name="start"></param>
        ''' <returns></returns>
        ''' <remarks>#6614</remarks>
        Private Function GetIndex(start As Integer) As Integer
            ClientLogUtil.Logger.DebugAP("QC001F04Form:GetIndex start")
            ClientLogUtil.Logger.DebugAP("QC001F04Form:GetIndex end")
            Return (CInt(txtGenzaiNoPage.Text) - 1) * CInt(txtIchiPageNoKensuu.Text) + start
        End Function

        ' ST1_#3010 横展開対応(QC001F04)
        ''' <summary>
        ''' 明細のキー操作制御
        ''' </summary>
        ''' <param name="spr">FpSpread</param>
        ''' <remarks>#3010</remarks>
        Private Sub setInputMapkeys(ByRef spr As FarPoint.Win.Spread.FpSpread)
            ClientLogUtil.Logger.DebugAP("QC001F04Form:setInputMapkeys start")
            Dim im As New FarPoint.Win.Spread.InputMap

            '非編集セルでの［Enter］キーを「次の列へ移動」とします
            im = spr.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused)
            im.Put(New FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap)

            ' 非編集セルでの[Tab]キーを「無効」とします
            im.Put(New FarPoint.Win.Spread.Keystroke(Keys.Tab, Keys.None), FarPoint.Win.Spread.SpreadActions.None)

            ' 非編集中セルでの[PageDown]キーを「無効」とします
            im.Put(New FarPoint.Win.Spread.Keystroke(Keys.PageDown, Keys.None), FarPoint.Win.Spread.SpreadActions.None)

            ' 非編集中セルでの[PageUp]キーを「無効」とします
            im.Put(New FarPoint.Win.Spread.Keystroke(Keys.PageUp, Keys.None), FarPoint.Win.Spread.SpreadActions.None)

            ' 非編集中セルでの［Enter］キーを「次の列へ移動」とします
            im = spr.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused)
            im.Put(New FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap)

            ' 編集中セルでの[Tab]キーを「無効」とします
            im.Put(New FarPoint.Win.Spread.Keystroke(Keys.Tab, Keys.None), FarPoint.Win.Spread.SpreadActions.None)

            ' 編集中セルでの[PageDown]キーを「無効」とします
            im.Put(New FarPoint.Win.Spread.Keystroke(Keys.PageDown, Keys.None), FarPoint.Win.Spread.SpreadActions.None)

            ' 編集中セルでの[PageUp]キーを「無効」とします
            im.Put(New FarPoint.Win.Spread.Keystroke(Keys.PageUp, Keys.None), FarPoint.Win.Spread.SpreadActions.None)
            ClientLogUtil.Logger.DebugAP("QC001F04Form:setInputMapkeys end")
        End Sub

        ''' <summary>
        ''' ページ可変明細SPREADのキー入力
        ''' </summary>
        ''' <remarks>#3010</remarks>
        Private Sub sprM1MenuIchiran_KeyDown(ByVal sender As Object, e As KeyEventArgs) Handles sprM1MenuIchiran.KeyDown

            ' [PageDown]キー
            If e.KeyCode = Keys.PageDown Then
                If btnTsugiPageHyouji.Enabled = True Then

                    ' [PageDown]キーを「次のページへ移動」とします
                    Dim sender2 = btnTsugiPageHyouji
                    Me.BtnNextPage_Click(sender2, e)
                Else

                    ' [PageDown]キーを「最後の列へ移動」とします
                    Dim pageSize As Integer = CInt(txtIchiPageNoKensuu.Text)
                    showActiveCell(sprM1MenuIchiran, pageSize - 1)

                End If
            End If

            ' [PageUp]キー
            If e.KeyCode = Keys.PageUp Then
                If btnMaePegeHyouji.Enabled = True Then

                    ' [PageUp]キーを「前のページへ移動」とします
                    Dim sender2 = btnMaePegeHyouji
                    Me.BtnPrePage_Click(sender2, e)
                Else

                    ' [PageDown]キーを「最初の列へ移動」とします
                    Dim pageSize As Integer = CInt(txtIchiPageNoKensuu.Text)
                    showActiveCell(sprM1MenuIchiran, 0)

                End If
            End If

            ' [Shift]キー
            Me.sprM1ShiftFlg = e.Shift
            ' [Ctrl]キー
            Me.sprM1CtrlFlg = e.Control

        End Sub

        Private Sub sprM1MenuIchiran_KeyUp(ByVal sender As Object, e As KeyEventArgs) Handles sprM1MenuIchiran.KeyUp

            ' [Shift]キー
            Me.sprM1ShiftFlg = e.Shift
            ' [Ctrl]キー
            Me.sprM1CtrlFlg = e.Control

        End Sub

        ''' <summary>
        ''' ページ不可変明細SPREADの[PageDown]と[PageUp]キー入力
        ''' </summary>
        ''' <remarks>#3010</remarks>
        Private Sub sprM2GokeiIchiran_KeyDown(ByVal sender As Object, e As KeyEventArgs) Handles sprM2GokeiIchiran.KeyDown

            ' [PageDown]キー
            If e.KeyCode = Keys.PageDown Then

                ' [PageDown]キーを「最後の列へ移動」とします
                showActiveCell(sprM2GokeiIchiran, sprM2GokeiIchiran.ActiveSheet.RowCount - 1)

            End If

            ' [PageUp]キー
            If e.KeyCode = Keys.PageUp Then

                ' [PageDown]キーを「最初の列へ移動」とします
                showActiveCell(sprM2GokeiIchiran, 0)

            End If

        End Sub

        ''' <summary>
        ''' 指定した「最初の行へ移動」か「最後の行へ移動」とします
        ''' </summary>
        ''' <param name="spr">FpSpread</param>
        ''' <param name="rowIndex">指定行index　0から</param>
        ''' <remarks>#3010</remarks>
        Private Sub showActiveCell(ByRef spr As FarPoint.Win.Spread.FpSpread， rowIndex As Integer)
            With spr
                If rowIndex < 0 Then
                    rowIndex = 0
                End If
                .ActiveSheet.SetActiveCell(rowIndex, .ActiveSheet.ActiveColumnIndex)
                .ShowActiveCell(VerticalPosition.Nearest, HorizontalPosition.Nearest)
            End With
        End Sub
        ' ST1_#3010 横展開対応(QC001F04)

        '#3450 Start
        ''' <summary>
        ''' ComboBoxドロップダウンリストの幅を設定
        ''' </summary>
        Private Sub ComboBoxWidth(ByRef combobox As ComboBox)
            For Each row As ComboxOptionDto In combobox.Items
                combobox.DropDownWidth = Math.Max(combobox.DropDownWidth, TextRenderer.MeasureText(row.Name, combobox.Font).Width)
            Next
        End Sub
        '#3450 End

        ''' <summary>
        ''' Ｍ２＿合計一覧行選択
        ''' </summary>
        Private Sub sprM2GokeiIchiran_CellClick(sender As Object, e As CellClickEventArgs) Handles sprM2GokeiIchiran.CellClick

            '#3018
            If e.Column = 0 Then
                e.Cancel = True
                sprM2GokeiIchiran.ActiveSheet.SetActiveCell(e.Row, 0)
                sprM2GokeiIchiran.ActiveSheet.ClearSelection()
                sprM2GokeiIchiran.ActiveSheet.AddSelection(e.Row, -1, 1, -1)
            End If

        End Sub

        ''' <summary>
        ''' 右クリック  ST#5744 ADD
        ''' </summary>
        Public Sub Migi_Click(ByVal sender As Object, ByVal e As MouseEventArgs) Handles sprM1MenuIchiran.MouseDown

            If e.Button <> MouseButtons.Right Then
                Exit Sub
            End If

            Dim currentRow As Integer = -1
            Dim currentCol As Integer = -1
            Dim activeCellRanges() As FarPoint.Win.Spread.Model.CellRange

            ' マウスポインタ位置の行番号の取得とコンテキストメニューの表示
            Dim htInfo As FarPoint.Win.Spread.HitTestInformation = sprM1MenuIchiran.HitTest(e.X, e.Y)
            If htInfo.Type = FarPoint.Win.Spread.HitTestType.RowHeader Then
                ' 行ヘッダの場合
                currentRow = -1
                currentCol = htInfo.ViewportInfo.Column
            ElseIf htInfo.Type = FarPoint.Win.Spread.HitTestType.Viewport Then
                ' 通常セルの場合
                currentRow = htInfo.ViewportInfo.Row
                currentCol = htInfo.ViewportInfo.Column
                ' セルの位置をクリックした場合
                If currentRow > -1 AndAlso currentCol > -1 Then

                    Dim startRow As Integer = sprM1MenuIchiran_Sheet1.Models.Selection.AnchorRow
                    Dim endRow As Integer = sprM1MenuIchiran_Sheet1.Models.Selection.LeadRow
                    Dim isChange As Boolean = True

                    ' 行選択が必要な行番号のリストを初期化
                    Dim selectRows As New List(Of Integer)

                    ' 現在の選択範囲を取得（複数の選択範囲が取得できる場合あり）
                    activeCellRanges = sprM1MenuIchiran.ActiveSheet.GetSelections()

                    ' 現在の選択範囲から行番号を取得
                    For Each activeCellRange As FarPoint.Win.Spread.Model.CellRange In activeCellRanges

                        For i = 0 To activeCellRange.RowCount() - 1
                            If activeCellRange.Row() + i = currentRow Then
                                currentRow = startRow
                                isChange = False
                                Exit For
                            End If
                        Next
                    Next

                    If isChange = True Then
                        sprM1MenuIchiran.ActiveSheet.SetActiveCell(currentRow, currentCol)
                    End If
                End If
            End If

            ' 参照モードの場合 明細未選択の場合
            If {Consts.SyoriMode.Reference}.Contains(SharedComClient.InstanceData.QC001F00FormDTO.HdnSyoriMode) OrElse
                currentRow < 0 Then
                ' 非活性
                ' 最新単価取込
                Me.RightClickSaishinTankaTorikomi.Enabled = False
                '#IT1-0085 削除 Start
                ' 複写
                'Me.RightClickCopy.Enabled = False
                '#IT1-0085 削除 End
                ' 切取
                Me.RightClickCut.Enabled = False
                ' 貼付
                Me.RightClickCopyPaste.Enabled = False
                ' 行挿入
                Me.RightClickInsert.Enabled = False
                ' 行削除
                Me.RightClickDelete.Enabled = False
            ElseIf String.IsNullOrWhiteSpace(sprM1MenuIchiran_Sheet1.Cells(currentRow, 1).Value) Then

                ' 最新単価取込
                Me.RightClickSaishinTankaTorikomi.Enabled = False
                '#IT1-0085 変更 Start
                ' 複写
                Me.RightClickCopy.Enabled = True
                '#IT1-0085 変更 End
                ' 切取
                Me.RightClickCut.Enabled = False
                ' 貼付
                Me.RightClickCopyPaste.Enabled = True
                ' 行挿入
                Me.RightClickInsert.Enabled = False
                ' 行削除
                If currentRow = sprM1MenuIchiran_Sheet1.RowCount - 1 AndAlso Not btnTsugiPageHyouji.Enabled Then
                    Me.RightClickDelete.Enabled = False
                Else
                    Me.RightClickDelete.Enabled = True
                End If
                For Each activeCellRange As FarPoint.Win.Spread.Model.CellRange In activeCellRanges
                    For i = 0 To activeCellRange.RowCount() - 1
                        If Not String.IsNullOrWhiteSpace(sprM1MenuIchiran_Sheet1.Cells(activeCellRange.Row + i, 1).Value) Then
                            ' 最新単価取込
                            Me.RightClickSaishinTankaTorikomi.Enabled = True
                            ' 複写
                            Me.RightClickCopy.Enabled = True
                            ' 切取
                            Me.RightClickCut.Enabled = True
                            Exit For
                        End If
                    Next
                Next
            Else
                ' 活性
                Me.RightClickSaishinTankaTorikomi.Enabled = True
                ' 複写
                Me.RightClickCopy.Enabled = True
                ' 切取
                Me.RightClickCut.Enabled = True
                ' 貼付
                Me.RightClickCopyPaste.Enabled = True

                If (String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Change) AndAlso
                    {"1", "3"}.Contains(SharedComClient.InstanceData.QC001F00FormDTO.KeiHenkouKaiyakuSyori)) OrElse
                    {Consts.KidoMode.Modify,
                    Consts.KidoMode.DemoKirikae,
                    Consts.KidoMode.DemoKasidasi,
                    Consts.KidoMode.Sinsei,
                    Consts.KidoMode.Print,
                    Consts.KidoMode.TanNendoUpdate}.Contains(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn) Then
                    ' 行挿入
                    Me.RightClickInsert.Enabled = False
                    ' 行削除
                    Me.RightClickDelete.Enabled = False
                Else
                    ' 行挿入
                    Me.RightClickInsert.Enabled = True
                    ' 行削除
                    Me.RightClickDelete.Enabled = True
                End If
            End If

            '# 9295 START 2022/11/04
            If (String.Equals(SharedComClient.InstanceData.QC001F00FormDTO.HdnKidoKbn, Consts.KidoMode.Modify)) Then
                ' 推奨構成検索 
                Me.RightClickSuishoKosei.Enabled = False
                ' 一括グループ設定
                Me.ToolStripMenuItem3.Enabled = False
                ' 最新単価取込
                Me.RightClickSaishinTankaTorikomi.Enabled = False
                ' 複写
                Me.RightClickCopy.Enabled = False
                ' 切取
                Me.RightClickCut.Enabled = False
                ' 貼付
                Me.RightClickCopyPaste.Enabled = False
            End If
            '# 9295 START 2022/11/04

        End Sub

        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　明細行をキーボード「↓」押下（新）　Start
        Private Sub downClickInsert_Row(sender As Object, e As PreviewKeyDownEventArgs) Handles sprM1MenuIchiran.PreviewKeyDown
            ClientLogUtil.Logger.DebugAP("QC001F04Form:downClickInsert_Row start")
            If e.KeyCode = Keys.Down Then
                ' 選択行の取得
                Me.GetRowIndex()
                '(1)-1.最終行を選択時に、空白行１行の追加を行う。
                If qc001F04FormDto.SelectedRowIndex.Count > 0 AndAlso
                    qc001F04FormDto.SelectedRowIndex.Last = Me.sprM1MenuIchiran_Sheet1.RowCount - 1 Then
                    qc001F04FormDto.SprM1MenuIchiran.Add(New QC001F04M1Dto())
                    Me.Paging()
                End If
            End If
            ClientLogUtil.Logger.DebugAP("QC001F04Form:downClickInsert_Row End")
        End Sub
        '仕様変更対応　QC001F04_見積・契約入力【たよ明細タブ】　明細行をキーボード「↓」押下（新）　End


        ''' <summary>
        ''' イベントハンドルを一時停止
        ''' </summary>
        Private Sub StopHandler()
            RemoveHandler cmbMeisaiHyojiSetteiSettisakiCombo.SelectedIndexChanged, AddressOf cmbMeisaiHyojiSetteiSettisakiCombo_SelectedIndexChanged
            RemoveHandler rdoGokeiHyojiSetteiHoshuRadio.CheckedChanged, AddressOf rdoGokeiHyojiSetteiHoshuRadio_Checked

            RemoveHandler sprM1MenuIchiran_Sheet1.CellChanged, AddressOf M1_LostFocus
            RemoveHandler rdoNebikiSetteiMenuBetsuRadio.CheckedChanged, AddressOf rdoNebikiSetteiMenuBetsuRadio_Change
            RemoveHandler cmbNebikiSetteiMarumeSetteiCombo.SelectedIndexChanged, AddressOf cmbNebikiSetteiMarumeSetteiCombo_LostFocus
            RemoveHandler sprM1MenuIchiran.SelectionChanged, AddressOf sprM1MenuIchiran_SelectionChanged

            ' ### ADD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
            RemoveHandler cmbHoshuKbnCombo.SelectedIndexChanged, AddressOf cmbHoshuKbnCombo_LostFocus
            RemoveHandler Me.Resize, AddressOf QC001F04_HeightChange
            RemoveHandler sprM2GokeiIchiran_Sheet1.CellChanged, AddressOf lblM2GokeiranNengakuHiyo_LostFocus
            RemoveHandler sprHoshuRyokinSansyutsuKijunDate.Resize, AddressOf Spread_Resize
            ' ### ADD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

        End Sub

        ''' <summary>
        ''' イベントハンドルを再開
        ''' </summary>
        Private Sub ResumeHandler()
            Me.StopHandler()
            AddHandler cmbMeisaiHyojiSetteiSettisakiCombo.SelectedIndexChanged, AddressOf cmbMeisaiHyojiSetteiSettisakiCombo_SelectedIndexChanged
            AddHandler rdoGokeiHyojiSetteiHoshuRadio.CheckedChanged, AddressOf rdoGokeiHyojiSetteiHoshuRadio_Checked

            AddHandler sprM1MenuIchiran_Sheet1.CellChanged, AddressOf M1_LostFocus
            AddHandler rdoNebikiSetteiMenuBetsuRadio.CheckedChanged, AddressOf rdoNebikiSetteiMenuBetsuRadio_Change
            AddHandler cmbNebikiSetteiMarumeSetteiCombo.SelectedIndexChanged, AddressOf cmbNebikiSetteiMarumeSetteiCombo_LostFocus
            AddHandler sprM1MenuIchiran.SelectionChanged, AddressOf sprM1MenuIchiran_SelectionChanged

            ' ### ADD-START KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）
            AddHandler cmbHoshuKbnCombo.SelectedIndexChanged, AddressOf cmbHoshuKbnCombo_LostFocus
            AddHandler Me.Resize, AddressOf QC001F04_HeightChange
            AddHandler sprM2GokeiIchiran_Sheet1.CellChanged, AddressOf lblM2GokeiranNengakuHiyo_LostFocus
            AddHandler sprHoshuRyokinSansyutsuKijunDate.Resize, AddressOf Spread_Resize
            ' ### ADD-END KATO 2022/09/02 性能改善（フォームロード中のイベント抑止）

        End Sub

        '#11622 参照モードのコントロール制御を追加 Start
        ''' <summary>
        ''' 画面初期制御（参照モード）
        ''' </summary>
        Private Sub InitControlReference()
            ClientLogUtil.Logger.DebugAP("QC001F04Form:InitControlReference start")

            Me.cmbHoshuKbnCombo.Enabled = False
            Me.sprHoshuRyokinSansyutsuKijunDate.ReadOnly = True
            Me.rdoNebikiSetteiMenuBetsuRadio.Enabled = False
            Me.rdoNebikiSetteiZidoAnbunRadio.Enabled = False
            Me.cmbNebikiSetteiMarumeSetteiCombo.Enabled = False
            Me.btnMenuSentaku.Enabled = False
            Me.btnSuishoKosei.Enabled = False
            Me.btnTaKyoten.Enabled = False
            Me.btnMenuFutai.Enabled = False
            Me.btnRyokinSaiKeisan.Enabled = False
            Me.btnFutaiNyuryoku.Enabled = False
            Me.btnSeigoCheck.Enabled = False
            'Me.sprM1MenuIchiran.Enabled = False '横スクロール等もできなくなるのでNG
            'Me.sprM1MenuIchiran.ActiveSheet.OperationMode = OperationMode.ReadOnly '#11622 差し戻し 背景グレーが必要なための方法はNG

            '#11622 差し戻し 背景グレーにしてロックする
            Dim bgColor = Drawing.Color.FromArgb(131, 131, 131)
            For row As Integer = 0 To Me.sprM1MenuIchiran.ActiveSheet.Rows.Count - 1
                For col As Integer = 0 To Me.sprM1MenuIchiran.ActiveSheet.ColumnCount - 1
                    With Me.sprM1MenuIchiran.ActiveSheet.Cells(row, col)
                        .Locked = True
                        .BackColor = bgColor
                    End With
                Next
            Next
            '#IT1-0085 変更 Start
            Me.btnCopy.Enabled = True
            '#IT1-0085 変更 End
            Me.btnCut.Enabled = False
            Me.btnPaste.Enabled = False
            Me.btnRowInsert.Enabled = False

            ClientLogUtil.Logger.DebugAP("QC001F04Form:InitControlReference end")
        End Sub
        '#11622 参照モードのコントロール制御を追加 End

        ''' <summary>
        ''' 画面初期制御（ステータス先行）
        ''' </summary>
        Private Sub InitControlStatus()
            '「画面制御パターン：参照のみ」と同じ
            InitControlReference()
            Me.btnGroupHenko.Enabled = False
        End Sub

        '#13555 ADD START 2022/10/04 QQ)K.Umino 並び替え時に選択したセルが隠れないよう修正
        ''' <summary>
        ''' 並び替え時選択セルに対して自動スクロールを行う
        ''' </summary>
        Private Sub ActiveCellScroll(selectIndex As Integer, changeTask As Integer)
            If changeTask = 0 Then
                If sprM1MenuIchiran.GetViewportTopRow(0) >= selectIndex Then
                    sprM1MenuIchiran.ShowActiveCell(VerticalPosition.Top, HorizontalPosition.Left)
                ElseIf sprM1MenuIchiran.GetViewportBottomRow(0) - 1 < selectIndex Then
                    '表示領域下部は少し見えている行があるため、1多い
                    sprM1MenuIchiran.ShowActiveCell(VerticalPosition.Bottom, HorizontalPosition.Left)
                End If
            ElseIf changeTask = 1 Then
                If sprM1MenuIchiran.GetViewportTopRow(0) > selectIndex Then
                    sprM1MenuIchiran.ShowActiveCell(VerticalPosition.Top, HorizontalPosition.Left)
                ElseIf sprM1MenuIchiran.GetViewportBottomRow(0) - 1 <= selectIndex Then
                    sprM1MenuIchiran.ShowActiveCell(VerticalPosition.Bottom, HorizontalPosition.Left)
                End If
            End If
        End Sub
        '#13555 ADD END   2022/10/04 QQ)K.Umino
#End Region

    End Class
End Namespace