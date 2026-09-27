using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Enums;

namespace SNUL.Shared.Persistance.Seeding
{
    public static class BogusDemoSeeder
    {
        public const string Marker = "BogusSeeder";
        private const int FakerSeed = 1234;

        private static readonly (string En, string Ar, string[] ChildEn, string[] ChildAr)[] Specialties =
        {
            ("Dental", "طب الأسنان",
                new[] { "Forceps", "Elevators", "Mirrors" },
                new[] { "ملاقط", "روافع", "مرايا" }),
            ("Orthopedic", "جراحة العظام",
                new[] { "Bone Saws", "Retractors", "Osteotomes" },
                new[] { "مناشير العظام", "مبعدات", "أزاميل العظام" }),
            ("Spine", "جراحة العمود الفقري",
                new[] { "Rongeurs", "Kerrisons", "Probes" },
                new[] { "قوارض", "ملاقط كيريسون", "مسابر" }),
            ("Plastic Surgery", "جراحة التجميل",
                new[] { "Scissors", "Needle Holders", "Calipers" },
                new[] { "مقصات", "حاملات الإبر", "فرجارات" }),
            ("CMF", "جراحة الوجه والفكين",
                new[] { "Mini Plates", "Screws", "Periosteals" },
                new[] { "صفائح مصغرة", "براغي", "روافع السمحاق" }),
            ("ENT", "الأنف والأذن والحنجرة",
                new[] { "Speculums", "Suction Tubes", "Curettes" },
                new[] { "مناظير", "أنابيب الشفط", "مكحتات" }),
        };

        private static readonly string[] InstrumentEn =
        {
            "Mayo Scissors", "Metzenbaum Scissors", "Kelly Forceps", "Needle Holder",
            "Army Retractor", "Scalpel Handle", "Towel Clamp", "Halsted Hemostat",
            "Bone Rongeur", "Periosteal Elevator", "Mouth Mirror", "Explorer Probe",
            "Nasal Speculum", "Osteotome Set", "Castroviejo Calipers", "Yankauer Suction",
            "Adson Forceps", "Iris Scissors", "Dental Syringe", "Bone File",
        };

        private static readonly string[] InstrumentAr =
        {
            "مشرط جراحي", "ملقط طبي", "مقص جراحي", "مبعد", "مسبار",
            "مبضع", "حامل إبر", "مرآة فم", "محقنة", "مبرد عظام",
            "أنبوب شفط", "فرجار قياس",
        };

        private static readonly string[] InstrumentModifiers =
            { "Standard", "Premium", "Deluxe", "Slim", "Curved", "Straight" };

        private static readonly string[] Materials =
            { "German Stainless Steel", "Titanium", "Tungsten Carbide", "AISI 420 Steel" };

        private static readonly string[] ProcedureTags =
        {
            "Oral Surgery", "Implantology", "Orthodontics", "Trauma",
            "Arthroscopy", "Spine Fusion", "Rhinoplasty", "Otology",
        };

        private static readonly (string Name, string ImageName, CompanyType Type)[] CompanySeedList =
        {
            ("Apex Surgical Supplies", "apex-surgical.svg", CompanyType.Distributor),
            ("MedCore Distributors", "medcore.svg", CompanyType.Distributor),
            ("Gulf Medical Trading", "gulf-medical.svg", CompanyType.Distributor),
            ("CityCare Hospitals Group", "citycare.svg", CompanyType.Hospital),
            ("Nova Health Clinic", "nova-health.svg", CompanyType.Clinic),
            ("PrimeCare Medical", "primecare.svg", CompanyType.Clinic),
            ("Sahara Med Import", "sahara-med.svg", CompanyType.Distributor),
            ("Delta Surgical Co.", "delta-surgical.svg", CompanyType.Distributor),
            ("LifeLine Hospitals", "lifeline.svg", CompanyType.Hospital),
            ("OrthoPlus Distributors", "orthoplus.svg", CompanyType.Distributor),
            ("CarePoint Clinics", "carepoint.svg", CompanyType.Clinic),
            ("Meridian Med Import", "meridian.svg", CompanyType.Distributor),
        };

        private static readonly string[] CompanyNames =
        {
            "Apex Surgical Supplies", "MedCore Distributors", "Gulf Medical Trading",
            "CityCare Hospitals Group", "Nova Health Clinic", "PrimeCare Medical",
            "Sahara Med Import", "Delta Surgical Co.", "LifeLine Hospitals",
            "OrthoPlus Distributors", "CarePoint Clinics", "Meridian Med Import",
        };

        private static readonly string[] OemServices =
            { "Private Label", "Custom Development", "Laser Marking", "Packaging", "Branding" };

        /// <summary>
        /// Joins help-article paragraphs with LF-only blank lines. The public Help Center
        /// splits article bodies on "\n\n", so a "\r\n\r\n" produced by a Windows verbatim
        /// string would not split and the whole body would render as one paragraph.
        /// </summary>
        private static string Para(params string[] paragraphs) => string.Join("\n\n", paragraphs);

        // The English category names are load-bearing: the seeder matches on them and
        // the admin UI is keyed off them, so they must stay byte-for-byte stable.
        private static readonly (string Name, string NameAr, string Icon)[] HelpCategories =
        {
            ("Ordering", "الطلبات", "shopping_cart"),
            ("Shipping & Incoterms", "الشحن وشروط التجارة الدولية", "local_shipping"),
            ("Returns & RMA", "المرتجعات وطلبات الاسترجاع", "restart_alt"),
            ("Sterilization", "التعقيم", "cleaning_services"),
            ("Warranty", "الضمان", "verified"),
        };

        private static readonly (string Category, string Slug, string Title, string TitleAr, string Body, string BodyAr)[] HelpArticles =
        {
            // --- Ordering ---
            ("Ordering", "requesting-a-quotation", "Requesting a quotation", "طلب عرض السعر",
                Para(
                    "Send us the instrument list you need and we will prepare a quotation. The most useful requests include the catalogue reference or product code, the quantity required for each item, and the country or port of delivery. If you are quoting for a tender, include the submission deadline so we can confirm whether we are able to meet it.",
                    "For instruments that are not in our catalogue, a short description or a photograph of the item is usually enough for us to identify it. Please state the intended surgical specialty and the working length you need, because the same instrument name is used for several lengths and curves.",
                    "Every quotation carries a stated validity period. Treat that period as the deadline for confirming quantities and shipping details, and ask us to extend it in writing if your internal approval process takes longer."),
                Para(
                    "أرسل لنا قائمة الأدوات التي تحتاجها وسنُعدّ لك عرض سعر. وتكون الطلبات الأكثر فائدة هي التي تتضمن مرجع الكتالوج أو رمز المنتج، والكمية المطلوبة من كل صنف، وبلد أو ميناء التوصيل. وإذا كنت تقدّم عرضاً لمناقصة، فيرجى ذكر الموعد النهائي لتقديم العروض حتى نتمكن من تأكيد قدرتنا على الوفاء به.",
                    "أما الأدوات غير الموجودة في كتالوجنا، فيكفي عادةً وصف موجز أو صورة للصنف للتعرف عليه. ويرجى ذكر التخصص الجراحي المقصود وطول العمل المطلوب، لأن الاسم الواحد يُستخدم لعدة أطوال وانحناءات مختلفة.",
                    "يحمل كل عرض سعر مدّة صلاحية مبيَّنة. عامل هذه المدّة على أنها الموعد النهائي لتأكيد الكميات وتفاصيل الشحن، واطلب تمديدها كتابةً إذا استغرقت إجراءات الاعتماد في جهتك وقتاً أطول.")),

            ("Ordering", "minimum-orders-and-trial-orders", "Minimums and trial orders", "الحد الأدنى للطلبات والطلبات التجريبية",
                Para(
                    "Minimum order quantities are set per item, not per shipment, and they vary by instrument and by whether the item is a stock configuration or a build-to-order item. Stock items ship from finished goods; build-to-order items enter production after the order is confirmed. We will tell you which case applies before you commit to a quantity.",
                    "If you are evaluating a new line, ask us for a mixed trial shipment that samples several references in small quantities. This is the usual way to test instruments without holding a full production quantity, and we will confirm the carton implications with it.",
                    "Custom lengths, private-label branding and non-standard finishing all affect the minimum and the lead time. Where a request falls below the minimum, we will say so plainly and offer the nearest workable alternative rather than quote something that cannot be produced."),
                Para(
                    "تُحدَّد الحد الأدنى للطلب لكل صنف على حدة، لا لكل شحنة، وتتغيّر بحسب الأداة وبخاصة ما إذا كانت الأداة من تشكيلة جاهزة أم تُصنَّع عند الطلب. الأدوات الجاهزة تُشحن من مخزون الإنتاج، أما الأدوات المصنَّعة عند الطلب فتدخل خط الإنتاج بعد تأكيد الطلب. وسنُخبرك بالحالة التي تنطبق قبل أن تلتزم بكمية محددة.",
                    "وإذا كنت تختبر خطاً جديداً، فاطلب منا شحنة تجريبية مختلطة تضمّ عدة مراجع بكميات صغيرة. وهذه هي الطريقة المعتادة لتجربة الأدوات دون الاحتفاظ بكمية إنتاج كاملة، وسنؤكّد لك أثر ذلك على حجم الكرتونة.",
                    "أما الأطوال الخاصة والشعارات الخاصة بالعميل والتشطيبات غير القياسية، فجميعها تؤثر في الحد الأدنى ومدة التجهيز. وإذا كان الطلب أقل من الحد الأدنى، سنقول ذلك بوضوح ونقترح أقرب بديل قابل للتنفيذ بدلاً من تقديم عرض لا يمكن تصنيعه.")),

            ("Ordering", "custom-instruments-and-branding", "Custom instruments and branding", "الأدوات الخاصة ووضع شعار العميل",
                Para(
                    "We produce to customer drawings and to modifications of catalogue items. A useful custom request states the reference, the modification, the reason for it, and the procedure in which the instrument will be used, so engineering can assess feasibility before quoting.",
                    "Branding options include laser marking on the instrument, printed marks on the packaging, and a supplied insert for the box. Marking is placed on an area selected for legibility and for cleanability, and we will confirm the marking artwork with you before it goes to production.",
                    "Development work is quoted separately from the instrument price. Where a modification is repeated across a set, or becomes a standing item for you, we will tell you, because the tooling and the setup cost are then shared rather than repeated."),
                Para(
                    "نصنّع الأدوات وفق رسومات العميل ووفق تعديلات على الأصناف الموجودة في الكتالوج. وطلب التطوير المفيد يذكر المرجع والتعديل المُراد وسببه والإجراء الجراحي الذي ستُستخدم فيه الأداة، ليتمكن الهندسة من دراسة جدوى التنفيذ قبل تقديم العرض.",
                    "وتشمل خيارات وضع العلامة التجارية حرق الليزر على الأداة نفسها، والعلامات المطبوعة على العبوة، وبطاقة إرشادية تُوضع في العلبة. ويُحدَّد موضع الحرق في منطقة مختارة للوضوح ولتسهيل التنظيف، وسنؤكّد معك تصميم العلامة قبل بدء الإنتاج.",
                    "يُقدَّم عمل التطوير بعرض سعر منفصل عن سعر الأداة. وإذا تكرّر التعديل في عدة أصوات، أو صار صنفاً دائماً لديك، فسنُعلمك بذلك، لأن تكاليف الأدوات والنقل تتقاسم حينها بدلاً من تكرارها في كل مرة.")),

            // --- Shipping & Incoterms ---
            ("Shipping & Incoterms", "incoterms-explained", "Incoterms explained", "شرح شروط التجارة الدولية (إنكوترمز)",
                Para(
                    "An Incoterm tells both sides where responsibility for the goods passes and who arranges carriage. It does not set the price and it does not replace the import obligations of your country. Read the term and the named place together, because EXW and FOB differ mainly in where the handover occurs.",
                    "The most common terms for a first export order are EXW, FOB and CIF. With EXW the buyer collects the goods from our works, so the buyer arranges everything. With FOB we load the goods on board at the named port and hand over the documents, and the buyer takes over from that point. With CIF we also arrange and pay for carriage to the named destination port, though insurance remains a separate matter to confirm.",
                    "If you have no in-house logistics team, tell us and we will quote the term that hands over the least to you and quote the carriage separately so you can see it as a line item. Tell us the destination port early, since it affects both the term and the packing.",
                    "Whichever term you choose, the risk and the cost of damage in transit are governed by the term, not by whatever the packing looks like on arrival. Inspect and photograph the pallets before signing the delivery note, and note any visible damage on it."),
                Para(
                    "شروط التجارة الدولية (إنكوترمز) تُبيّن للطرفين عند نقطة التسليم تتحول المسؤولية عن البضاعة ومن يتولى ترتيب النقل. وهي لا تحدّد السعر ولا تحل محل الالتزامات الجمركية في بلدك. اقرأ الشرط مع اسم المكان المذكور، فالفرق بين EXW وFOB يكمن أساساً في مكان التسليم.",
                    "والشروط الأكثر شيوعاً في أول طلب تصدير هي EXW وFOB وCIF. وفي حال EXW يستلم المشتري البضاعة من مصنعنا، أي يتولى كل شيء. أما في FOB فنحمل البضاعة على متن الوسيلة في الميناء المذكور ونسلّم المستندات، ويتولى المشتري المسؤولية من تلك اللحظة. أما في CIF فنحن نرتب وندفع أجرة النقل حتى ميناء الوجهة المذكور، مع أن التأمين يظل أمراً منفصلاً يجب تأكيده.",
                    "وإذا لم يكن لديك فريق لوجستي داخلي، فأخبرنا وسنُقدّم عرضاً بالشرط الذي ينقل إليك أقل قدر من المسؤوليات، مع تسعير النقل بشكل منفصل لتظهره كبند مستقل. وأخبرنا بميناء الوجهة مبكراً، لأنه يؤثر في الشرط وفي التغليف معاً.",
                    "وأيًّا كان الشرط الذي تختاره، فإن المخاطر وتكلفة التلف أثناء النقل يحكمها الشرط نفسه، لا مظهر التغليف عند الوصول. افحص المنصات وصوّرها قبل التوقيع على سند التسليم، ودوّن أي تلف ظاهر عليه.")),

            ("Shipping & Incoterms", "export-documents", "Export documents for customs clearance", "مستندات التصدير لتخليص الجمارك",
                Para(
                    "Every export shipment is accompanied by a commercial invoice and a packing list that agree with each other on quantities, weights and carton numbers. The invoice states the agreed unit prices, the Incoterm and the named place, and it is the document your customs authority will use to assess duty.",
                    "For instruments classified as medical devices, your import authority may also ask for a certificate of conformity, a declaration of conformity, or evidence that the manufacturer holds a recognised quality management certification. We supply the documents we hold on request; ask early in the process rather than at the port.",
                    "The commercial invoice and packing list are usually issued in English, and we can provide a bilingual English-Arabic version on request. Your import licence, if your country requires one, is your responsibility to obtain before shipment; we cannot ship against an import licence that has not been issued.",
                    "Keep the airway bill number, the commercial invoice number and the packing list together when the shipment arrives. Most clearance questions are answered faster from those three references than from a description of the contents."),
                Para(
                    "يصحب كل شحنة تصدير فاتورة تجارية وقائمة تعبئة متفقتان فيما بينهما في الكميات والأوزان وأرقام الكراتين. وتذكر الفاتورة أسعار الوحدات المتفق عليها والشرط التجاري واسم المكان، وهي المستند الذي ستستخدمه سلطتك الجمركية لتقدير الرسوم.",
                    "وبالنسبة للأدوات المصنَّفة كأجهزة طبية، قد تطلب جهة الاستيراد لديك أيضاً شهادة مطابقة أو إقراراً بالمطابقة أو إثباتاً بأن المصنع حاصل على اعتماد لنظام إدارة جودة معترف به. ونقدّم المستندات المتوفرة لدينا عند الطلب؛ فاطلبها مبكراً في إجراءات الاستيراد، لا عند الميناء.",
                    "تصدر الفاتورة التجارية وقائمة التعبئة عادةً بالإنجليزية، ويمكننا توفير نسخة ثنائية اللغة بالإنجليزية والعربية عند الطلب. أما ترخيص الاستيراد، إن كان مطلوباً في بلدك، فالحصول عليه قبل الشحن مسؤوليتك؛ ولا يمكننا الشحن بناءً على ترخيص لم يُصدر بعد.",
                    "احتفظ برقم بوليصة الشحن ورقم الفاتورة التجارية وقائمة التعبئة معاً عند وصول الشحنة. فمعظم استفسارات التخليص تُجاب بسرعة أكبر بهذه المراجع الثلاثة مقارنةً بوصف المحتوى.")),

            ("Shipping & Incoterms", "cartons-pallets-and-consolidation", "Cartons, pallets and consolidation", "الكراتين والمنصات وتجميع الشحنات",
                Para(
                    "Instruments are supplied in inner packaging, then in export cartons, then on pallets. Carton quantities are chosen by us to protect the instruments in transit rather than to fill a container, so a set that is comfortable loose may be supplied in more, smaller cartons.",
                    "Palletising depends on the transport mode. Sea freight is palletised and often shrink-wrapped; air freight is usually shipped as cartons on an air pallet, and the gross weight and the height per piece drive the cost. Tell us the mode and the destination port early and we will confirm the packing plan before production.",
                    "When several orders are ready at once we can consolidate them into one consignment. Consolidation reduces the freight cost per instrument but makes the paperwork for each underlying order its own set of documents, and it means a shortage in one order can hold the whole consignment. We will tell you when that trade-off applies.",
                    "If you are receiving several pallets, check them against the packing list on arrival and photograph any damage before signing. Instruments that arrive with a crushed or wet carton should be reported before the consignment is put into general stock."),
                Para(
                    "تُورَّد الأدوات في عبوات داخلية، ثم في كراتين التصدير، ثم على المنصات. وتُختار كميات الكراتين من جانبنا بما يحمي الأدوات أثناء النقل، لا لملء الحاوية؛ لذا قد يأتي طقم يسهل نقله منفردين ونورّده في كراتين أكثر وأصغر.",
                    "يعتمد وضع الأدوات على المنصة حسب وسيلة النقل. الشحن البحري يُحمَّل على منصات ويُغلَّف غالباً بمادة انكماش، أما الشحن الجوي فيُرسَل عادةً كراتين على منصة شحن جوي، حيث يقود الوزن الإجمالي والارتفاع لكل قطعة تكلفة الشحنة. أخبرنا بوسيلة النقل وميناء الوجهة مبكراً وسنؤكّد خطة التغليف قبل الإنتاج.",
                    "وعند جاهزية عدة طلبات في وقت واحد يمكننا تجميعها في شحنة واحدة. ويخفض التجميع أجرة الشحن لكل أداة، لكنه يجعل لكل طلب من الطلبات الأساسية مجموعته الخاصة من المستندات، كما أن نقصاً في طلب واحد قد يؤخر الشحنة كاملة. وسنُعلمك عند انطباق هذه المفاضلة.",
                    "وإذا استلمت عدة منصات، فقارنها بقائمة التعبئة عند الوصول وصوّر أي تلف قبل التوقيع. والأدوات التي تصل في كرتون مهروس أو مبلل يجب الإبلاغ عنها قبل إدخال الشحنة إلى المخزون العام.")),

            // --- Returns & RMA ---
            ("Returns & RMA", "when-an-rma-is-required", "When an RMA is required", "متى يلزم تقديم طلب استرجاع",
                Para(
                    "A return authorization request is the only accepted way to send instruments back to us. Please do not return anything without it. An unannounced return is difficult to identify on arrival, cannot be matched to a claim, and will be held in goods-in until we can establish what it is, which delays any credit or replacement.",
                    "We need an RMA for a manufacturing defect, for damage in transit that was not recorded on the delivery note, for an incorrect item in the consignment, or for a shortfall against the packing list. Each of these needs different evidence, so raising the right request first is what keeps the case moving.",
                    "Change of mind, a surplus stock and a discontinued line are not defects and are not covered by the RMA process. If you are overstocked, contact us before you do anything with the instruments, because a surplus that can be worked into a later order is a much easier conversation than one that has already been shipped back."),
                Para(
                    "طلب الإذن بالإرجاع هو الطريقة الوحيدة المقبولة لإرسال الأدوات إلينا. يرجى عدم إرجاع أي شيء قبل الحصول عليه. فالإرجاع غير المُعلَن يصعب التعرّف عليه عند الوصول، ولا يمكن مطابقته بأي مطالبة، وسيُحتجز في قسم الاستلام حتى نتمكن من تحديد ما هو، مما يؤخّر أي ردّ قيمة أو استبدال.",
                    "نحتاج إلى طلب إرجاع في حال وجود عيب تصنيع، أو تلف أثناء النقل لم يُدوَّن على سند التسليم، أو صنف غير مطابق في الشحنة، أو نقص مقارنةً بقائمة التعبئة. ولكل حالة منها أدلة مختلفة، لذا فإن رفع الطلب المناسب أولاً هو ما يحافظ على سير الحالة.",
                    "أما تغيّر الرأي، أو فائض المخزون، أو إيقاف خط إنتاج، ليست عيوباً ولا يشملها إجراء الإرجاع. وإذا كان لديك مخزون فائض، فتواصل معنا قبل أن تفعل أي شيء بالأدوات، لأن الفائض الذي يمكن استثماره في طلب لاحق أسهل بكثير في النقاش من فائض شُحن بالفعل.")),

            ("Returns & RMA", "rma-process-step-by-step", "The RMA process step by step", "خطوات تقديم طلب الاسترجاع",
                Para(
                    "First, record the instrument reference, the lot or serial marking, the purchase order or invoice number, and the date the problem was found. Then photograph the instrument and, where relevant, the packaging and the shipping label. Send these with your RMA request to our support address.",
                    "Our quality team reviews the request and issues an RMA number with the address to use. Do not use a different address: sending to the works without a number means the parcel is handled as an unidentified delivery.",
                    "Pack the instruments for the return journey in the same protective manner as the original shipment, or better, and declare the value as the original invoice value. Include a copy of the RMA number inside the carton. We will confirm receipt and the disposition, which is either a repair, a replacement, a credit note or a return to you at your cost.",
                    "Keep the RMA number and the outcome together in your quality records. A repeated defect on the same reference is a trend, and a trend is much easier to act on than a series of isolated claims that nobody has connected."),
                Para(
                    "أولاً: سجّل مرجع الأداة وعلامة التشغيلة أو الرقم التسلسلي، ورقم أمر الشراء أو الفاتورة، وتاريخ اكتشاف المشكلة. ثم صوّر الأداة، وعند الاقتضاء صوّر العبوة وبطاقة الشحن. وأرسل هذه المستندات مع طلب الإرجاع إلى عنوان الدعم لدينا.",
                    "يراجع فريق الجودة الطلب ويصدر رقم إرجاع مع العنوان الواجب استخدامه. ولا تستخدم عنواناً آخر؛ فالإرسال إلى المصنع دون رقم يجعل الطرد مُعاملاً كطرد مجهول الهوية.",
                    "غُلِّف الأدوات ل رحلة العودة بالطريقة الوقائية نفسها التي غُلِّفت بها عند الشحن الأصلي، أو بطريقة أفضل، وصرّح بالقيمة المطابقة لقيمة الفاتورة الأصلية. وضع نسخة من رقم الإرجاع داخل الكرتون. وسنؤكّد الاستلام والمصير، وهو أحد أربعة: إصلاح، أو استبدال، أو إشعار دائن، أو إعادتها إليك على نفقتك.",
                    "احتفظ برقم الإرجاع والنتيجة معاً في سجلات الجودة لديك. فالعيب المتكرر في المرجع نفسه اتجاهاً، والاتجاه أسهل في التعامل معه من سلسلة شكاوى متفرقة لم يربط بينها أحد.")),

            ("Returns & RMA", "replacing-defective-instruments", "Replacing a defective instrument", "استبدال الأداة المعيبة",
                Para(
                    "Not every defect means a replacement. A manufacturing defect is assessed against the intended use and the condition in which the instrument arrived. Items that can be brought back to specification by our workshop are repaired, because that returns a working instrument to you faster and keeps it in clinical use.",
                    "Where an item cannot be brought back to specification, or where the same reference has failed repeatedly, we replace it. The decision and the reason are recorded on the RMA, so you have a written basis for the decision in your own file.",
                    "We will also tell you when a failure was caused by handling rather than by manufacturing, and what to change to avoid it: reprocessing temperature, contact with incompatible cleaning agents, or exceeding the instrument's working life. That conversation is usually more valuable than the replacement itself.",
                    "Keep returned items out of clinical stock even after the RMA is raised. An instrument that has been sent for assessment should not go back to a tray, because you cannot use it while its condition is being examined."),
                Para(
                    "ليس كل عيب يستدعي الاستبدال. فالعيب التصنيعي يُقيَّم بحسب الغرض من الاستخدام والحالة التي وصلت بها الأداة. والأصناف التي يمكن إعادتها إلى المواصفة بإصلاحها في ورشتنا نصلحها، لأن ذلك يعيد لك أداة صالحة للاستخدام أسرع ويُبقيها في الخدمة السريرية.",
                    "وعندما لا يمكن إعادة الصنف إلى مواصفته، أو عندما يتعطّل المرجع نفسه مراراً، نستبدله. ويُسجَّل القرار وسببه في طلب الإرجاع، فتحصل على أساس مكتوب للقرار في ملفك.",
                    "وسنُخبرك أيضاً عندما يكون سبب العطل ناتجاً عن التعامل لا عن التصنيع، وبما ينبغي تغييره لتجنبه: كحرارة إعادة التعقيم، أو ملامسة مواد تنظيف غير متوافقة، أو تجاوز العمر التشغيلي للأداة. وهذه المحادثة غالباً أنفع من الاستبدال نفسه.",
                    "أخرج الأصناف المُعادة من المخزون السريري حتى بعد رفع طلب الإرجاع. فلا يجوز أن تعود أداة أُرسلت للتقييم إلى صينية العمل، لأنك لا تستطيع استخدامها أثناء فحص حالتها.")),

            // --- Sterilization ---
            ("Sterilization", "autoclave-reprocessing", "Autoclave reprocessing at 134 °C", "إعادة التعقيم بالأوتوكلاف عند 134 درجة مئوية",
                Para(
                    "Reusable surgical instruments supplied for steam sterilization are intended to be reprocessed in a pre-vacuum steam autoclave at 134 °C. The cycle must include a drying stage, because residual moisture in a hollow or boxed instrument is the most common cause of both staining and corrosion.",
                    "Exposure time, not just temperature, has to be correct for the cycle you are running, and it has to be validated in your own sterilizer with your own load. Do not take the cycle time from this page or from a general table; take it from your validated protocol.",
                    "Instruments must be fully clean and dry before they enter the autoclave. Soil, blood, protein residue and salt are all harder to sterilise than the instrument surface, and they also cause the pitting and staining that make a good instrument look used when it is not."),
                Para(
                    "الأدوات الجراحية القابلة لإعادة الاستخدام والموردة للتعقيم بالبخار مُعدّة لإعادة المعالجة في أوتوكلاف بخار مسبوق بالتفريغ عند درجة حرارة 134 درجة مئوية. ويجب أن تتضمن الدورة مرحلة تجفيف، لأن الرطوبة المتبقية في الأدوات المجوّفة أو المغلّفة هي السبب الأكثر شيوعاً للبقع والتآكل معاً.",
                    "لا تكفي الحرارة وحدها؛ فزمن التعرّض، إلى جانبها، يجب أن يكون صحيحاً لدورة التعقيم التي تشغّلها، ويجب أن يكون مُتحقَّقاً منه في جهازك أنت وبحمولتك أنت. لا تأخذ زمن الدورة من هذه الصفحة ولا من جدول عام؛ خذه من بروتوكولك المعتمَد.",
                    "يجب أن تكون الأدوات نظيفة وجافة تماماً قبل إدخالها الأوتوكلاف. فالأوساخ والدم وبقايا البروتين والملح كلها أصعب في التعقيم من سطح الأداة نفسه، وهي أيضاً سبب التنقّط والبقع التي تجعل أداة سليمة تبدو مستعملة وهي ليست كذلك.")),

            ("Sterilization", "cleaning-before-sterilization", "Cleaning and disinfection before sterilization", "التنظيف والتطهير قبل التعقيم البخاري",
                Para(
                    "Cleaning comes before sterilization, always. Instruments are rinsed or wiped as soon as possible after use, because dried protein is progressively harder to remove. Follow your facility's protocol for pre-cleaning, and never soak stainless instruments in solutions that are not intended for stainless steel.",
                    "Use a neutral-pH, enzymatic cleaner and a soft brush or a cleaning basket. Harsh alkaline cleaners, chloride-containing agents and abrasive pads attack the passive layer on the steel, and the corrosion they start continues in service long after the instruments leave your department.",
                    "Check each instrument as it is cleaned: hinge movement, box locks, screw heads, serrations and cutting edges. A hinge that has stiffened, or a ratchet that no longer engages fully, is a defect and should be reported rather than worked around in theatre.",
                    "Dry instruments before packaging, and dry the box interiors. Moisture trapped in a box joint or in a basket is the starting point for wet-pack failures, which your sterilizer will flag even when the instruments themselves have been correctly treated."),
                Para(
                    "يأتي التنظيف قبل التعقيم، دائماً. تُشطف الأدوات أو تُمسح في أقرب وقت ممكن بعد الاستخدام، لأن البروتين المجفّ يصعب إزالته تدريجياً أكثر كلما طال الوقت. واتّبع بروتوكول مرافقك في التنظيف الأولي، ولا تنقع الأدوات المصنوعة من الفولاذ المقاوم للصدأ في محاليل غير مخصصة له.",
                    "استخدم منظفاً إنزيمياً متعادل الحموضة مع فرشاة ناعمة أو سلة تنظيف. فالمنظفات القلوية القاسية والمحاليل التي تحتوي كلوريداً واللبادات الكاشطة تهاجم الطبقة السلبية على الفولاذ، والتآكل الذي تبدأه هذه العوامل يستمر أثناء الاستخدام بعد أن تغادر الأدوات قسمك بوقت طويل.",
                    "افحص كل أداة أثناء تنظيفها: حركة المفصلة، والأقفال، ورؤوس البراغي، والتسنونات، وحافّات القطع. فالمفصلة التي صلبت، أو الترس الذي لم يعد يشتبك بالكامل، يُعدّ عيباً ويجب الإبلاغ عنه لا تخطيط له في غرفة العمليات.",
                    "جفّف الأدوات قبل تغليفها، وجفّف داخل العلب. والرطوبة المحتبسة عند مفصل العلبة أو في السلة هي نقطة بداية أعطال البلل، التي سيُنبّهك إليها جهاز التعقيم حتى لو عولجت الأدوات نفسها معالجة صحيحة.")),

            ("Sterilization", "packaging-and-indicators", "Packaging, indicators and shelf life", "التغليف والمؤشرات ومدة الصلاحية",
                Para(
                    "Instruments are supplied non-sterile, in individual peel packs or in inner bags, and are expected to be placed into your own validated sterile barrier system. Packaging is not a substitute for a validated container or pouch, and it does not carry a sterile status on its own.",
                    "Use chemical indicators inside every pack, and an external indicator on the outside of the container. Indicators show that a process reached conditions, not that the load was sterile, and they are read against the colour change specified for the indicator you are using. Reading an indicator is a competence, not a formality.",
                    "Sterile barrier systems have a shelf life that is set by the manufacturer of the packaging and the validated storage conditions, and it is not a universal number. Storage outside the validated conditions, or beyond the expiry, invalidates the pack regardless of how the indicator looks.",
                    "Rotate stock oldest-first and keep the load separation rules in mind when you build the autoclave load, because overfilling reduces the steam's ability to reach every surface. If you are ever unsure whether a pack should be used, the pack must be treated as non-sterile."),
                Para(
                    "تُورَّد الأدوات غير معقمة، في عبوات فردية قابلة للكشف أو في أكياس داخلية، ويُتوقع أن توضع في نظام حاجز معقّم معتمَد خاص بك. فالتغليف ليس بديلاً عن حاوية أو كيس معتمد، ولا يحمل بذاته حالة التعقيم.",
                    "استخدم مؤشراً كيميائياً داخل كل عبوة، ومؤشراً خارجياً على خارج الحاوية. والمؤشرات تُبيّن أن العملية بلغت الظروف المطلوبة، لا أن الحمولة أصبحت معقمة، وتُقرأ المؤشرات مقابل تغيّر اللون المحدَّد للمؤشر الذي تستخدمه. وقراءة المؤشر مهارة لا شكيلة.",
                    "لأنظمة الحاجز المعقّم مدة صلاحية يحدّدها مصنع التغليف وشروط التخزين المعتمدة، وليست رقماً عاماً موحّداً. والتخزين خارج الشروط المعتمدة، أو بعد انتهاء المدّة، يُبطل صلاحية العبوة مهما كان شكل المؤشر.",
                    "ادور المخزون من الأقدم إلى الأحدث، وخذ قواعد فصل الحمولات في الحسبان عند بناء حمولة الأوتوكلاف، لأن الإزدحام يقلل قدرة البخار على الوصول إلى كل سطح. وإذا شككت يوماً في صلاحية عبوة ما، فيجب معاملتها على أنها غير معقمة.")),

            // --- Warranty ---
            ("Warranty", "warranty-coverage", "What the warranty covers", "ما الذي يشمله الضمان",
                Para(
                    "The warranty covers manufacturing defects in the material and in the workmanship of an instrument supplied by us. A defect means the instrument does not perform to its stated specification under normal use and normal reprocessing. It does not mean the instrument has worn out, and wear is not a defect.",
                    "The warranty runs from the date shown on your invoice. It applies to the instrument itself, not to the procedure it was used in and not to the outcome of that procedure. Keeping the original invoice, the delivery note and the batch or serial marking is what allows a claim to be matched to a production run.",
                    "A claim is assessed on the instrument and on the evidence you provide, not on how frequently you use the item. A surgical instrument that has been in daily use for years and fails is treated the same as one that has been used twice, provided the defect is genuine and the reprocessing history is sound.",
                    "Where a defect is genuine, we repair or replace. Where the instrument has been altered, sharpened by a third party, or reprocessed outside the published parameters, we will say so in the assessment, because that changes what we can honestly conclude from it."),
                Para(
                    "يشمل الضمان عيوب التصنيع في مادة الأداة وفي جودة تشغيلها. والعيب يعني أن الأداة لا تعمل وفق مواصفتها المعلنة في الاستخدام الطبيعي وفي إعادة المعالجة الطبيعية، ولا يعني أن الأداة قد استهلكت؛ فالاستهلاك ليس عيباً.",
                    "يمتد الضمان من التاريخ المدوَّن على فاتورتك. وهو يسري على الأداة نفسها، لا على الإجراء الذي استُخدمت فيه، ولا على نتيجة ذلك الإجراء. والاحتفاظ بالفاتورة الأصلية وسند التسليم وعلامة التشغيلة أو الرقم التسلسلي هو ما يسمح بمطابقة أي مطالبة بدفعة إنتاج معيّنة.",
                    "تُقيَّم المطالبة على أساس الأداة والأدلة التي تقدّمها، لا على أساس تكرار استخدام الصنف. فأداة جراحية مستخدمة يومياً منذ سنوات ومعطوبة تُعامَل على قدم المساواة مع أخرى استُخدمت مرتين، ما دام العيب حقيقياً وسجل إعادة المعالجة سليم.",
                    "وعندما يكون العيب حقيقياً، فإننا نصلح أو نستبدل. أما إذا كانت الأداة قد عُدِّلت أو شُحِّذت لدى طرف ثالث، أو أُعيدت معالجتها خارج المعايير المعلنة، فسنذكر ذلك في التقييم، لأنه يغيّر ما يمكن استخلاصه منها بأمان.")),

            ("Warranty", "making-a-warranty-claim", "Making a warranty claim", "تقديم مطالبة بالضمان",
                Para(
                    "Contact us with the invoice reference, the instrument reference and a description of the fault in the words you would use with a technician. Photographs or a short video of the fault are worth more than a description alone, and a photograph of the batch or serial marking is what lets us trace the production run.",
                    "Send the instrument back under a return authorization so that it reaches the right place and is recorded against your case. Do not send it to the address on your delivery note unless we have told you to, and do not send a loose instrument in an envelope: the damage that causes in transit usually ends the claim.",
                    "We will confirm the assessment in writing, including whether the item is repaired, replaced or credited. If the assessment needs a third-party opinion, for example a metallurgical check on a fractured instrument, we will say that this is what is happening and why it takes the time it takes.",
                    "If a claim is declined, you will be given the reason and, where it helps, what would have made it assessable. Please treat that as information rather than as a refusal to look again, and come back to us if the situation changes."),
                Para(
                    "تواصل معنا مرفقاً مرجع الفاتورة ومرجع الأداة ووصفاً للعطل بالعبارات التي تستخدمها مع فني الصيانة. والصور أو مقطع فيديو قصير للعطل أنفع من الوصف وحده، أما صورة علامة التشغيلة أو الرقم التسلسلي فهي ما يتيح لنا تتبّع دفعة الإنتاج.",
                    "أرسل الأداة في إطار إذن إرجاع حتى تصل إلى الوجهة الصحيحة وتُسجَّل مقابل حالتك. ولا ترسلها إلى العنوان المدوَّن على سند التسليم إلا إذا طلبنا منك ذلك، ولا ترسل أداة منفردة في مظروف؛ فالتلف الذي يلحقها أثناء النقل يُنهي المطالبة عادةً.",
                    "سنؤكّد نتيجة التقييم كتابةً، بما فيها ما إذا كان الصنف سيُصلَح أو يُستبدل أو يُخصم. وإذا احتاج التقييم إلى رأي طرف ثالث، مثل فحص معدني لأداة مكسورة، فسنوضح أن هذا ما يجري ولماذا يستغرق ما يستغرقه من وقت.",
                    "وإذا رُفضت مطالبة، فسيُعطى لك السبب، وعند الإمكان ما كان يمكن أن يجعلها قابلة للتقييم. ويُرجى التعامل مع ذلك على أنه معلومة لا رفضاً للنظر من جديد، فراجع إلينا إذا تغيّرت الأحوال.")),

            ("Warranty", "what-warranty-excludes", "What the warranty does not cover", "ما لا يشمله الضمان",
                Para(
                    "Normal wear is not covered. Cutting edges dull, box locks loosen, serrations round off and pivot screws take up, and these are the expected consequences of use rather than defects. Sharpeners, blade replacements and spring replacements are maintenance items and are treated as such.",
                    "Damage caused by reprocessing outside the published parameters is not covered. This includes exceeding the stated steam temperature, using chlorine-containing or acidic cleaning agents on stainless steel, and leaving instruments wet in a closed box. The corrosion that follows looks identical to corrosion that started as a manufacturing fault, and only the history distinguishes them.",
                    "Loss or theft in transit, and damage from a consignment that was not inspected on arrival, are governed by the Incoterm and by your own insurance rather than by the warranty. Neither is damage caused by shipping the instruments loose, unprotected, or in a case that was not intended for the purpose.",
                    "Instruments modified, relabelled or serviced by a party other than us are outside the warranty for the modification and for any consequential damage, because we can no longer vouch for the instrument as it leaves our hands. The unmodified parts of the same instrument remain covered."),
                Para(
                    "الاستهلاك الطبيعي غير مشمول. فحوافّ القطع تفقد حدّها، والأقفال ترتخي، والتسنونات تستدقّ، وبراغي المحور تتخلخل، وهذه نتائج متوقعة للاستخدام لا عيوب. أما صقل الحوافّ وتبديل الشفرات واستبدال الزنبركات فهي بنود صيانة تُعامل على هذا الأساس.",
                    "التلف الناتج عن إعادة المعالجة خارج المعايير المعلنة غير مشمول. ويشمل ذلك تجاوز حرارة البخار المحدَّدة، واستخدام محاليل تحتوي كلوراً أو أحماضاً على الفولاذ المقاوم للصدأ، وترك الأدوات مبللة في علبة مغلقة. والتآكل الذي ينتج عن ذلك يبدو مطابقاً للتآكل الذي بدأ عيباً تصنيعياً، ولا يميّز بينهما إلا التاريخ.",
                    "الفقد أو السرقة أثناء النقل، والتلف الناتج عن شحنة لم تُفحص عند وصولها، فيحكمهما الشرط التجاري (إنكوترم) وتأمينك أنت لا الضمان. وينطبق الأمر أيضاً على التلف الناتج عن شحن الأدوات منفردة أو بلا حماية أو في حزمة غير مخصصة للغرض.",
                    "الأدوات التي يعدّلها أو يعيد وسمها أو يخدمها طرف غيرنا خارج الضمان بالنسبة لذلك التعديل وأي ضرر ناتج عنه، لأننا لم نعد نضمن الأداة كما تخرج من أيدينا. أما الأجزاء غير المعدّلة من الأداة نفسها فتبقى مشمولة.")),
        };

        private static readonly (string Question, string QuestionAr, string Answer, string AnswerAr)[] HelpFaqs =
        {
            ("Do you ship to my country?",
             "هل تشحنون إلى بلدي؟",
             "We ship to most destinations. Send us the destination country and, where you know it, the port or city of delivery, and we will confirm what we can do and which term applies. Some destinations need an import licence or a registration with the local health authority before we can ship, so tell us early and we will check.",
             "نشحن إلى معظم الوجهات. أرسل لنا بلد الوجهة، وميناء التوصيل أو مدينته إن كان معروفاً لديك، وسنؤكّد ما يمكننا فعله والشرط المنطبق. وبعض الوجهات تتطلب ترخيص استيراد أو تسجيلاً لدى الجهة الصحية المحلية قبل أن نتمكن من الشحن، لذا أخبرنا مبكراً وسنتحقق من ذلك."),

            ("What do you need from me to quote a price?",
             "ما المعلومات المطلوبة مني لتقديم عرض سعر؟",
             "The instrument reference or product code, the quantity per item and the destination. If you do not have a reference, a description with the intended specialty and the working length is enough for us to identify most items. Nothing is quoted from a catalogue line alone, because the length, the finish and the packaging all change the price.",
             "مرجع الأداة أو رمز المنتج، والكمية المطلوبة من كل صنف، والوجهة. وإذا لم يتوفر لديك مرجع، فإن وصفاً يذكر التخصص المقصود وطول العمل يكفي للتعرّف على معظم الأصناف. ولا يُقدَّم أي عرض سعر اعتماداً على سطر واحد من الكتالوج، لأن الطول والتشطيب والتغليف جميعها تؤثر في السعر."),

            ("Can you put our name or logo on the instruments?",
             "هل يمكن وضع اسم شركتنا أو شعارها على الأدوات؟",
             "Yes. We can laser-mark the instrument, print on the packaging, or supply an insert for the box. Tell us at the quotation stage rather than after production, because marking artwork has to be approved and prepared before the items are made. Marking is placed on an area chosen for legibility and cleanability, so it is not always where you would put it on a drawing.",
             "نعم. يمكننا حرق اسمكم أو شعاركم على الأداة بالليزر، أو طباعته على العبوة، أو توفير بطاقة إرشادية تُوضع في العلبة. وأخبرنا في مرحلة عرض السعر لا بعد الإنتاج، لأن تصميم العلامة يحتاج إلى اعتماد وتجهيز قبل تصنيع الأصناف. ويُحدَّد موضع العلامة في منطقة مختارة للوضوح ولتسهيل التنظيف، ولذلك قد لا يكون الموضع الذي تختارونه في الرسمة هو نفسه."),

            ("Are the instruments supplied sterile?",
             "هل تُسلَّم الأدوات معقمة؟",
             "No. Reusable instruments are supplied non-sterile so that your own facility controls the reprocessing, which is what makes the process auditable. They arrive individually packed, and you place them into your validated sterile barrier system before use.",
             "لا. تُورَّد الأدوات القابلة لإعادة الاستخدام غير معقمة، حتى تتحكم مرافقتك أنت في إعادة المعالجة، وهو ما يجعل العملية قابلة للتدقيق. وتصلك الأدوات في عبوات فردية، ثم تضعها في نظام الحاجز المعقّم المعتمد لديك قبل الاستخدام."),

            ("Can these instruments be autoclaved at 134 °C?",
             "هل يمكن تعقيم هذه الأدوات بالأوتوكلاف عند 134 درجة مئوية؟",
             "Reusable instruments intended for steam sterilization can be reprocessed at 134 °C in a pre-vacuum autoclave, provided the cycle includes a drying stage and the exposure time is taken from your own validated protocol. We cannot quote a cycle time for your machine: that is a function of your sterilizer, your load and your validation, not of our catalogue.",
             "يمكن إعادة معالجة الأدوات القابلة لإعادة الاستخدام والمخصَّصة للتعقيم بالبخار عند 134 درجة مئوية في أوتوكلاف مسبوق بالتفريغ، شريطة أن تتضمن الدورة مرحلة تجفيف وأن يُؤخذ زمن التعرّض من بروتوكولك المعتمَد. ولا يمكننا تحديد زمن دورة لجهازك، فذلك يتوقف على جهازك وحمولتك وتحققك، لا على كتالوجنا."),

            ("What does EXW mean for me?",
             "ماذا يعني شرط EXW بالنسبة لي؟",
             "EXW means the goods are made available at our works and the buyer takes over from that point. Under EXW you arrange and pay for everything downstream: loading, export clearance, carriage, insurance and import clearance in your country. It is the term that gives us the least involvement, which also means the least help if something goes wrong in transit.",
             "يعني شرط EXW أن البضاعة تكون متاحة في مصنعنا ويتولى المشتري المسؤولية من تلك النقطة. وبموجب EXW تتولى أنت ترتيب ودفع كل ما يأتي بعدها: التحميل والتخليص للتصدير والنقل والتأمين والتخليص للاستيراد في بلدك. وهو الشرط الذي يمنحنا أقل قدر من التدخل، وبالتالي أقل قدر من المساعدة إذا حدث خطأ أثناء النقل."),

            ("Who arranges the freight and who pays for it?",
             "من ينظّم الشحنات ومن يدفع تكاليفها؟",
             "It depends on the Incoterm on your order confirmation. Under EXW and FOB you arrange the freight and you pay the carrier directly. Under CIF we arrange and pay the carriage to the named destination port. We will tell you which applies before the order is confirmed, and we will not change the term after the goods have been shipped.",
             "يعتمد ذلك على الشرط التجاري الوارد في تأكيد الطلب. وبموجب EXW وFOB تتولى أنت ترتيب الشحن وتدفع للناقل مباشرة. أما بموجب CIF فنحن نرتب وندفع أجرة النقل حتى ميناء الوجهة المذكور. وسنُخبرك بالشرط المنطبق قبل تأكيد الطلب، ولن نغيّر الشرط بعد شحن البضاعة."),

            ("How do I report a defective instrument?",
             "كيف أبلّغ عن أداة معيبة؟",
             "Send us the invoice reference, the instrument reference, the batch or serial marking and a description or photograph of the fault, and raise a return authorization so we can record the case. Do not return the instrument without an authorization number, because an unannounced return cannot be matched to a claim and will be held in goods-in.",
             "أرسل لنا مرجع الفاتورة ومرجع الأداة وعلامة التشغيلة أو الرقم التسلسلي ووصفاً أو صورة للعطل، وارفع طلب إذن إرجاع حتى نتمكن من تسجيل الحالة. ولا تُرجع الأداة دون رقم إذن إرجاع، لأن الإرجاع غير المُعلَن لا يمكن مطابقته بمطالبة وسيُحتجز في قسم الاستلام."),

            ("Can we start with a small trial order?",
             "هل يمكن البدء بطلب تجريبي صغير؟",
             "Yes, and it is the usual way to evaluate a new line. Ask for a mixed trial shipment sampling several references in small quantities rather than a full production quantity of one item. Minimums are set per item, so a mixed small shipment is more often possible than a small shipment of a single reference.",
             "نعم، وهذه هي الطريقة المعتادة لتقييم خط جديد. اطلب شحنة تجريبية مختلطة تضمّ عدة مراجع بكميات صغيرة بدلاً من كمية إنتاج كاملة لصنف واحد. فالحد الأدنى يُحدَّد لكل صنف، ما يجعل الشحنة الصغيرة المختلطة ممكنة أكثر من شحنة صغيرة لصنف واحد."),

            ("How long does production take?",
             "كم تستغرق فترة الإنتاج؟",
             "Lead time depends on whether the item is a stock configuration or is built to order, and on the size of the order. Stock items ship from finished goods; build-to-order items go into production after the order is confirmed, and custom or branded items take longer again because of artwork and setup. We confirm a lead time in writing with each quotation and update you if it changes.",
             "تعتمد مدة التجهيز على ما إذا كان الصنف من تشكيلة جاهزة أم يُصنَّع عند الطلب، وعلى حجم الطلب. فالأصناف الجاهزة تُشحن من مخزون الإنتاج، أما المصنَّعة عند الطلب فتدخل خط الإنتاج بعد تأكيد الطلب، وتستغرق الأصناف الخاصة أو ذات العلامة وقتاً أطول بسبب تصميم العلامة وعمليات التجهيز. ونؤكّد مدة التجهيز كتابةً مع كل عرض سعر، ونُعلمك إذا تغيّرت."),
        };

        private static string Slugify(string value)
        {
            var s = (value ?? string.Empty).Trim().ToLowerInvariant().Replace(' ', '-').Replace('_', '-');
            var sb = new System.Text.StringBuilder(s.Length);
            foreach (var ch in s)
                if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') || ch == '-')
                    sb.Append(ch);
            var slug = sb.ToString();
            while (slug.Contains("--")) slug = slug.Replace("--", "-");
            return slug.Trim('-');
        }

        private static List<T> PickSome<T>(Faker faker, IList<T> source, int min, int max)
        {
            if (source.Count == 0) return new List<T>();
            var upper = Math.Min(max, source.Count);
            var n = upper <= min ? source.Count : faker.Random.Int(min, upper);
            return faker.PickRandom(source, n).Distinct().ToList();
        }

        private static string DemoPassword(IConfiguration? config = null)
        {
            var fromConfig = config?["Seeding:DemoPassword"];
            if (!string.IsNullOrWhiteSpace(fromConfig))
                return fromConfig!;
            return Environment.GetEnvironmentVariable("SEED_DEMO_PASSWORD") is { Length: > 0 } pwd
                ? pwd
                : "Demo123!";
        }

        public static bool ShouldSeedDemoData(IHostEnvironment? env, IConfiguration? config, out string reason)
        {
            if (env != null && env.IsProduction())
            {
                reason = "refused: Production environment (Bogus demo data never runs in Production).";
                return false;
            }

            var fromConfig = config?.GetValue<bool>("Seeding:SeedDemoData") == true;
            var fromEnvVar = string.Equals(
                Environment.GetEnvironmentVariable("SEED_DEMO_DATA"),
                "true", StringComparison.OrdinalIgnoreCase);

            var fromConfigString = string.Equals(
                            config?["SEED_DEMO_DATA"],
                            "true", StringComparison.OrdinalIgnoreCase);

            if (fromConfig || fromEnvVar || fromConfigString)
            {
                reason = fromConfig
                    ? "enabled via Seeding:SeedDemoData=true."
                    : "enabled via SEED_DEMO_DATA=true.";
                return true;
            }

            reason = "skipped: set \"Seeding\": { \"SeedDemoData\": true } in appsettings.Development.json "
                + "(or user-secrets / --Seeding:SeedDemoData true) or SEED_DEMO_DATA=true env var.";
            return false;
        }

        public static async Task SeedDemoAsync(IServiceProvider services, CancellationToken ct = default)
        {
            try
            {
                var env = services.GetService<IHostEnvironment>();
                var config = services.GetService<IConfiguration>();
                if (!ShouldSeedDemoData(env, config, out var gateReason))
                {
                    return;
                }

                var db = services.GetRequiredService<SnulDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                var roleManager = services.GetService<RoleManager<IdentityRole<Guid>>>();
                if (roleManager != null)
                    await RoleSeeder.SeedRolesAsync(roleManager);

                Randomizer.Seed = new Random(FakerSeed);
                var faker = new Faker("en");
                var year = DateTime.UtcNow.Year;

                var usd = await db.Currencies.FirstOrDefaultAsync(c => !c.IsDeleted && c.Code == "USD", ct)
                                    ?? await db.Currencies.FirstOrDefaultAsync(c => !c.IsDeleted, ct);
                if (usd == null)
                {
                    await CurrencySeeder.SeedAsync(db);
                    usd = await db.Currencies.FirstOrDefaultAsync(c => !c.IsDeleted && c.Code == "USD", ct)
                        ?? await db.Currencies.FirstOrDefaultAsync(c => !c.IsDeleted, ct);
                }

                var countries = await db.Countries.Where(c => !c.IsDeleted).ToListAsync(ct);
                if (countries.Count == 0)
                {
                    await WorldLocationSeeder.SeedAsync(db);
                    countries = await db.Countries.Where(c => !c.IsDeleted).ToListAsync(ct);
                }
                if (countries.Count == 0)
                {
                    return;
                }

                List<Category> leaves;
                if (await db.Categories.AnyAsync(c => !c.IsDeleted, ct))
                {
                    leaves = await db.Categories.Where(c => !c.IsDeleted && c.ParentCategoryId != null).ToListAsync(ct);
                    if (leaves.Count == 0)
                        leaves = await db.Categories.Where(c => !c.IsDeleted).ToListAsync(ct);
                }
                else
                {
                    var categories = new List<Category>();
                    foreach (var (en, ar, childEn, childAr) in Specialties)
                    {
                        var root = Category.Create(en, ar, $"{en} surgical instruments", null, null, Marker);
                        categories.Add(root);
                        for (var i = 0; i < childEn.Length; i++)
                            categories.Add(Category.Create($"{en} {childEn[i]}", $"{ar} - {childAr[i]}", null, null, root.Id, Marker));
                    }
                    await db.Categories.AddRangeAsync(categories, ct);
                    await db.SaveChangesAsync(ct);
                    leaves = categories.Where(c => c.ParentCategoryId.HasValue).ToList();
                }
                List<Product> products;
                if (await db.Products.AnyAsync(p => !p.IsDeleted, ct))
                {
                    products = await db.Products.Where(p => !p.IsDeleted).Take(200).ToListAsync(ct);
                }
                else
                {
                    products = new List<Product>();
                    var specs = new List<ProductSpecification>();
                    var media = new List<ProductMedia>();
                    var tags = new List<ProductProcedureTag>();
                    var seq = 0;
                    foreach (var leaf in leaves)
                    {
                        for (var i = 0; i < 5; i++)
                        {
                            seq++;
                            var nameEn = $"{faker.PickRandom(InstrumentEn)} {faker.PickRandom(InstrumentModifiers)}";
                            var sku = $"WL-{100000 + seq}";
                            var slug = $"{Slugify(nameEn)}-{seq}";
                            var price = Math.Round(faker.Random.Decimal(15, 2500), 2);
                            var length = faker.Random.Bool(0.7f) ? (decimal?)Math.Round(faker.Random.Decimal(10, 30), 1) : null;
                            var material = faker.PickRandom(Materials);
                            var product = Product.Create(
                                nameEn,
                                $"{faker.PickRandom(InstrumentAr)} {seq}",
                                sku, slug,
                                faker.Lorem.Sentence(8, 4),
                                price,
                                faker.Random.Int(0, 300),
                                $"Autoclave 134°C; DIN 1.4021{(length.HasValue ? $"; {length} cm" : string.Empty)}",
                                $"demo/products/{slug}.jpg",
                                material, length,
                                usd?.Id,
                                leaf.Id, null, Marker);
                            products.Add(product);
                            specs.Add(new ProductSpecification { Id = Guid.NewGuid(), ProductId = product.Id, AttrName = "Material", AttrValue = material });
                            specs.Add(new ProductSpecification { Id = Guid.NewGuid(), ProductId = product.Id, AttrName = "Sterilization", AttrValue = "Autoclave 134°C" });
                            specs.Add(new ProductSpecification { Id = Guid.NewGuid(), ProductId = product.Id, AttrName = "Length", AttrValue = length.HasValue ? $"{length} cm" : "Standard" });
                            foreach (var s in specs.TakeLast(3)) s.MarkAsCreated(Marker);
                            media.Add(new ProductMedia { Id = Guid.NewGuid(), ProductId = product.Id, Type = ProductMediaType.Image, Url = $"demo/products/{slug}.jpg", SortOrder = 0 });
                            media.Last().MarkAsCreated(Marker);
                            if (faker.Random.Bool(0.3f))
                            {
                                media.Add(new ProductMedia { Id = Guid.NewGuid(), ProductId = product.Id, Type = ProductMediaType.Image, Url = $"demo/products/{slug}-2.jpg", SortOrder = 1 });
                                media.Last().MarkAsCreated(Marker);
                            }
                            foreach (var tag in faker.PickRandom(ProcedureTags, faker.Random.Int(1, 2)).Distinct())
                            {
                                var t = new ProductProcedureTag { Id = Guid.NewGuid(), ProductId = product.Id, Label = tag };
                                t.MarkAsCreated(Marker);
                                tags.Add(t);
                            }
                        }
                    }
                    await db.Products.AddRangeAsync(products, ct);
                    await db.ProductSpecifications.AddRangeAsync(specs, ct);
                    await db.ProductMedias.AddRangeAsync(media, ct);
                    await db.SaveChangesAsync(ct);
                }

                List<Company> companies;
                if (await db.Companies.AnyAsync(c => !c.IsDeleted, ct))
                {
                    companies = await db.Companies.Where(c => !c.IsDeleted).Take(20).ToListAsync(ct);
                    var updatedExisting = false;
                    for (var idx = 0; idx < companies.Count; idx++)
                    {
                        var comp = companies[idx];
                        if (!comp.IsProvider)
                        {
                            comp.IsProvider = true;
                            updatedExisting = true;
                        }
                        if (string.IsNullOrWhiteSpace(comp.ImageName))
                        {
                            var seedMatch = CompanySeedList.FirstOrDefault(s => s.Name.Equals(comp.Name, StringComparison.OrdinalIgnoreCase));
                            comp.ImageName = seedMatch.ImageName ?? CompanySeedList[idx % CompanySeedList.Length].ImageName;
                            updatedExisting = true;
                        }
                    }
                    if (updatedExisting)
                    {
                        await db.SaveChangesAsync(ct);
                    }
                }
                else
                {
                    companies = new List<Company>();
                    for (var i = 0; i < CompanySeedList.Length; i++)
                    {
                        var seed = CompanySeedList[i];
                        var country = faker.PickRandom(countries);
                        var status = i == CompanySeedList.Length - 1 ? CompanyStatus.Pending : CompanyStatus.Approved;
                        var company = Company.Create(
                            seed.Name, seed.Type, country.Id,
                            status, null, Marker,
                            $"info@{Slugify(seed.Name)}.example.com",
                            seed.ImageName);

                        company.IsProvider = true;
                        companies.Add(company);
                    }
                    await db.Companies.AddRangeAsync(companies, ct);
                    await db.SaveChangesAsync(ct);

                    var addresses = new List<CompanyAddress>();
                    foreach (var company in companies)
                    {
                        var n = faker.Random.Int(1, 2);
                        for (var a = 0; a < n; a++)
                        {
                            var country = countries.FirstOrDefault(c => c.Id == company.CountryId) ?? faker.PickRandom(countries);
                            var city = await db.Cities.FirstOrDefaultAsync(c => !c.IsDeleted && c.CountryId == country.Id, ct)
                                ?? await db.Cities.FirstOrDefaultAsync(c => !c.IsDeleted, ct);
                            if (city == null) continue;
                            var zone = await db.Zones.FirstOrDefaultAsync(z => !z.IsDeleted && z.CityId == city.Id, ct)
                                ?? await db.Zones.FirstOrDefaultAsync(z => !z.IsDeleted, ct);
                            if (zone == null) continue;
                            addresses.Add(CompanyAddress.Create(
                                company.Id, country.Id, city.Id, zone.Id,
                                $"{faker.Random.Int(1, 200)} {faker.Address.StreetName()}",
                                faker.Random.Bool(0.5f) ? $"Bldg {faker.Random.Int(1, 50)}" : null,
                                faker.Random.Bool(0.4f) ? $"Fl {faker.Random.Int(1, 20)}" : null,
                                faker.Random.Bool(0.4f) ? $"Apt {faker.Random.Int(1, 100)}" : null,
                                Marker, isDefault: a == 0));
                        }
                    }
                    await db.CompanyAddresses.AddRangeAsync(addresses, ct);
                    await db.SaveChangesAsync(ct);
                }

                var password = DemoPassword(config);
                var staff = new List<ApplicationUser>();
                for (var i = 1; i <= 2; i++)
                {
                    var u = await EnsureUserAsync(userManager, faker,
                        $"demo.staff{i:00}@snul.health", faker.Name.FullName(),
                        UserType.SnulStaff, null, i % 2 == 0 ? AppLanguage.Ar : AppLanguage.En,
                        password, ct);
                    if (u != null) staff.Add(u);
                }
                var orgUsers = new List<(ApplicationUser User, Company Company)>();
                var approved = companies.Where(c => c.Status == CompanyStatus.Approved).ToList();

                // Mediator model: spread catalog items across the approved
                // provider companies so provider-scoped reads ("my catalog",
                // "offered by") return real demo data. Products left with a
                // null CompanyId stay Admin-managed global items.
                if (approved.Count > 0)
                {
                    var unowned = await db.Products
                        .Where(p => !p.IsDeleted && p.CompanyId == null)
                        .OrderBy(p => p.CreatedAt)
                        .ToListAsync(ct);
                    for (var i = 0; i < unowned.Count; i++)
                    {
                        unowned[i].CompanyId = approved[i % approved.Count].Id;
                        unowned[i].MarkAsUpdated(Marker);
                    }
                    if (unowned.Count > 0)
                        await db.SaveChangesAsync(ct);
                }
                for (var i = 0; i < approved.Count; i++)
                {
                    var u = await EnsureUserAsync(userManager, faker,
                        $"demo.org{i + 1:00}@snul.health", faker.Name.FullName(),
                        UserType.OrganizationUser, approved[i].Id, AppLanguage.En, password, ct);
                    if (u != null) orgUsers.Add((u, approved[i]));

                    var u2 = await EnsureUserAsync(userManager, faker,
                        $"demo.member{i + 1:00}@snul.health", faker.Name.FullName(),
                        UserType.OrganizationUser, approved[i].Id, i % 2 == 0 ? AppLanguage.Ar : AppLanguage.En, password, ct);
                    if (u2 != null) orgUsers.Add((u2, approved[i]));
                }


                if (staff.Count == 0)
                    staff = await db.ApplicationUsers.Where(u => !u.IsDeleted && u.UserType == UserType.SnulStaff).Take(5).ToListAsync(ct);
                if (orgUsers.Count == 0 && approved.Count > 0)
                {
                    var orgDbUsers = await db.ApplicationUsers.Where(u => !u.IsDeleted && u.CompanyId != null).ToListAsync(ct);
                    foreach (var ou in orgDbUsers)
                    {
                        var comp = approved.FirstOrDefault(c => c.Id == ou.CompanyId);
                        if (comp != null) orgUsers.Add((ou, comp));
                    }
                }

                var activeMembers = orgUsers.Select(x => x.User).ToList();
                if (activeMembers.Count == 0)
                    activeMembers = await db.ApplicationUsers.Where(u => !u.IsDeleted && u.UserType == UserType.OrganizationUser).Take(20).ToListAsync(ct);
                if (activeMembers.Count == 0)
                    activeMembers = await db.ApplicationUsers.Where(u => !u.IsDeleted).Take(20).ToListAsync(ct);

                var membersWithAddress = new HashSet<Guid>(
                                    await db.UserAddresses.Where(a => !a.IsDeleted).Select(a => a.UserId).ToListAsync(ct));
                var memberAddresses = new List<UserAddress>();
                foreach (var c in activeMembers)
                {
                    if (!membersWithAddress.Contains(c.Id))
                    {
                        var country = faker.PickRandom(countries);
                        var city = await db.Cities.FirstOrDefaultAsync(x => !x.IsDeleted && x.CountryId == country.Id, ct)
                            ?? await db.Cities.FirstOrDefaultAsync(x => !x.IsDeleted, ct);
                        if (city == null) continue;
                        var zone = await db.Zones.FirstOrDefaultAsync(z => !z.IsDeleted && z.CityId == city.Id, ct)
                            ?? await db.Zones.FirstOrDefaultAsync(z => !z.IsDeleted, ct);
                        if (zone == null) continue;
                        memberAddresses.Add(UserAddress.Create(c.Id, country.Id, city.Id, zone.Id,
                            $"{faker.Random.Int(1, 200)} {faker.Address.StreetName()}", null, null, null, Marker, isDefault: true));
                    }
                }
                if (memberAddresses.Count > 0)
                {
                    await db.UserAddresses.AddRangeAsync(memberAddresses, ct);
                    await db.SaveChangesAsync(ct);
                }

                var repId = staff.Count > 0 ? staff[0].Id : Guid.NewGuid();
                var chainNo = 0;
                var chainPrefix = $"WO-{year}-";
                if (await db.Orders.AnyAsync(o => !o.IsDeleted && (o.CreatedBy == Marker || o.OrderNumber.StartsWith(chainPrefix)), ct))
                {
                }
                else
                {
                    foreach (var (user, company) in orgUsers.Take(products.Count > 0 ? 12 : 0))
                    {
                        chainNo++;
                        var items = PickSome(faker, products, 2, 5);
                        var rfq = new RFQ
                        {
                            Id = Guid.NewGuid(),
                            RFQNumber = $"RFQ-{year}-{chainNo:0000}",
                            CompanyId = company.Id,
                            Status = RFQStatus.Ordered,
                            AssignedSalesRepId = staff.Count > 0 ? repId : null,
                        };
                        rfq.MarkAsCreated(Marker);
                        foreach (var p in items)
                        {
                            var ri = new RFQItem
                            {
                                Id = Guid.NewGuid(),
                                RFQId = rfq.Id,
                                ProductId = p.Id,
                                Quantity = faker.Random.Int(5, 200),
                                UnitPrice = p.Price,
                                Notes = faker.Random.Bool(0.3f) ? "Urgent delivery requested" : null,
                            };
                            ri.MarkAsCreated(Marker);
                            rfq.Items.Add(ri);
                        }
                        var quoteTotal = Math.Round(rfq.Items.Sum(i => i.Quantity * i.UnitPrice) * (decimal)faker.Random.Double(0.92, 1.05), 2);
                        var quote = new Quote
                        {
                            Id = Guid.NewGuid(),
                            QuoteNumber = $"QT-{year}-{chainNo:0000}",
                            RFQId = rfq.Id,
                            Amount = quoteTotal,
                            ValidUntil = DateTime.UtcNow.AddDays(30),
                            Status = QuoteStatus.Approved,
                            CreatedBySalesRepId = repId,
                        };
                        quote.MarkAsCreated(Marker);
                        foreach (var ri in rfq.Items)
                        {
                            var qi = new QuoteItem
                            {
                                Id = Guid.NewGuid(),
                                QuoteId = quote.Id,
                                ProductId = ri.ProductId,
                                Quantity = ri.Quantity,
                                UnitPrice = ri.UnitPrice,
                            };
                            qi.MarkAsCreated(Marker);
                            quote.Items.Add(qi);
                        }
                        var order = new Order
                        {
                            Id = Guid.NewGuid(),
                            OrderNumber = $"WO-{year}-{chainNo:0000}",
                            Status = (OrderStatus)faker.Random.Int(2, 4),
                            UserId = user.Id,
                            CompanyId = company.Id,
                            CurrencyId = usd?.Id,
                            QuoteId = quote.Id,
                            TotalAmount = quoteTotal,
                            SnapshotBaseCurrency = "USD",
                            SnapshotCurrencyCode = usd?.Code ?? "USD",
                            SnapshotRate = 1,
                            SnapshotRateDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            SnapshotSource = Marker,
                        };
                        order.MarkAsCreated(Marker);
                        foreach (var qi in quote.Items)
                        {
                            var oi = new OrderItem
                            {
                                Id = Guid.NewGuid(),
                                OrderId = order.Id,
                                ProductId = qi.ProductId,
                                Quantity = qi.Quantity,
                                UnitPrice = qi.UnitPrice,
                            };
                            oi.MarkAsCreated(Marker);
                            order.Items.Add(oi);
                        }
                        var invoice = new Invoice
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            InvoiceNumber = $"INV-{year}-{chainNo:0000}",
                            Amount = quoteTotal,
                            Status = faker.Random.Bool(0.6f) ? InvoiceStatus.Paid : InvoiceStatus.Issued,
                        };
                        invoice.MarkAsCreated(Marker);
                        order.Invoices.Add(invoice);

                        await db.RFQs.AddAsync(rfq, ct);
                        await db.Quotes.AddAsync(quote, ct);
                        await db.Orders.AddAsync(order, ct);
                    }
                    await db.SaveChangesAsync(ct);
                }

                // Bilingual help content. The public Help Center renders in Arabic, so every
                // row carries both languages; the Arabic columns are nullable so a row that
                // only has English still loads and falls back instead of failing.
                //
                // These three guards keep the seeder idempotent. An earlier version of this
                // seeder wrote Lorem Ipsum bodies under slugs like "ordering-part-1", and
                // those rows are still present in the shared database. Because the guards
                // below are presence checks, re-running the seeder will NOT overwrite them.
                // Placeholder rows are identifiable by CreatedBy = 'BogusSeeder'. To load
                // the real content, clear them once and re-seed:
                //
                //   DELETE FROM HelpArticles WHERE CreatedBy = 'BogusSeeder';
                //   DELETE FROM FAQItems     WHERE CreatedBy = 'BogusSeeder';
                //   DELETE FROM HelpCategories WHERE CreatedBy = 'BogusSeeder';
                //
                // Deleting the categories is safe: HelpArticles cascades from them. Do it in
                // that order or the article delete will fail on the foreign key.
                var catEntities = new List<HelpCategory>();
                if (!await db.HelpCategories.AnyAsync(c => !c.IsDeleted, ct))
                {
                    foreach (var def in HelpCategories)
                    {
                        var hc = new HelpCategory { Id = Guid.NewGuid(), Name = def.Name, NameAr = def.NameAr, Icon = def.Icon };
                        hc.MarkAsCreated(Marker);
                        catEntities.Add(hc);
                    }
                    await db.HelpCategories.AddRangeAsync(catEntities, ct);
                    await db.SaveChangesAsync(ct);
                }
                else
                {
                    catEntities = await db.HelpCategories.Where(c => !c.IsDeleted).Take(10).ToListAsync(ct);
                }
                if (!await db.HelpArticles.AnyAsync(a => !a.IsDeleted, ct) && catEntities.Count > 0)
                {
                    var articles = new List<HelpArticle>();
                    foreach (var def in HelpArticles)
                    {
                        // Match on the English category name: it is the stable key shared with
                        // the definitions above, and it is what the admin UI displays.
                        var cat = catEntities.FirstOrDefault(c => c.Name == def.Category);
                        if (cat == null) continue;
                        var ha = new HelpArticle
                        {
                            Id = Guid.NewGuid(),
                            CategoryId = cat.Id,
                            Title = def.Title,
                            TitleAr = def.TitleAr,
                            Body = def.Body,
                            BodyAr = def.BodyAr,
                            Slug = def.Slug,
                        };
                        ha.MarkAsCreated(Marker);
                        articles.Add(ha);
                    }
                    await db.HelpArticles.AddRangeAsync(articles, ct);
                    await db.SaveChangesAsync(ct);
                }
                if (!await db.FAQItems.AnyAsync(f => !f.IsDeleted, ct))
                {
                    var faqs = new List<FAQItem>();
                    for (var i = 0; i < HelpFaqs.Length; i++)
                    {
                        var def = HelpFaqs[i];
                        var f = new FAQItem
                        {
                            Id = Guid.NewGuid(),
                            Question = def.Question,
                            QuestionAr = def.QuestionAr,
                            Answer = def.Answer,
                            AnswerAr = def.AnswerAr,
                            SortOrder = i + 1,
                        };
                        f.MarkAsCreated(Marker);
                        faqs.Add(f);
                    }
                    await db.FAQItems.AddRangeAsync(faqs, ct);
                    await db.SaveChangesAsync(ct);
                }
                if (!await db.ProductInquiries.AnyAsync(p => !p.IsDeleted, ct) && products.Count > 0)
                {
                    var inquiries = Enumerable.Range(1, 6).Select(_ =>
                    {
                        var p = faker.PickRandom(products);
                        var pi = new ProductInquiry
                        {
                            Id = Guid.NewGuid(),
                            ProductId = p.Id,
                            Name = faker.Name.FullName(),
                            Organization = faker.Company.CompanyName(),
                            Message = faker.Lorem.Paragraph(1),
                            Email = faker.Internet.Email(),
                        };
                        pi.MarkAsCreated(Marker);
                        return pi;
                    }).ToList();
                    await db.ProductInquiries.AddRangeAsync(inquiries, ct);
                    await db.SaveChangesAsync(ct);
                }
                if (!await db.OemInquiries.AnyAsync(o => !o.IsDeleted, ct))
                {
                    var oem = OemServices.Take(4).Select(s =>
                    {
                        var o = new OemInquiry
                        {
                            Id = Guid.NewGuid(),
                            FullName = faker.Name.FullName(),
                            Email = faker.Internet.Email(),
                            CompanyName = faker.Company.CompanyName(),
                            ServiceType = s,
                            Message = faker.Lorem.Paragraph(1),
                        };
                        o.MarkAsCreated(Marker);
                        return o;
                    }).ToList();
                    await db.OemInquiries.AddRangeAsync(oem, ct);
                    await db.SaveChangesAsync(ct);
                }

                if (!await db.SupportTickets.AnyAsync(t => !t.IsDeleted, ct) && activeMembers.Count > 0)
                {
                    var ticketUsers = activeMembers.Take(8).ToList();
                    var statuses = new[] { "Open", "Answered", "Closed" };
                    var tickets = new List<SupportTicket>();
                    foreach (var tu in ticketUsers)
                    {
                        var st = faker.PickRandom(statuses);
                        var t = new SupportTicket
                        {
                            Id = Guid.NewGuid(),
                            UserId = tu.Id,
                            Subject = faker.Lorem.Sentence(5, 2).TrimEnd('.'),
                            Message = faker.Lorem.Paragraph(1),
                            Status = st,
                            Reply = st == "Open" ? null : faker.Lorem.Paragraph(1),
                            RepliedAt = st == "Open" ? null : DateTime.UtcNow.AddDays(-faker.Random.Int(1, 9)),
                            RepliedBy = st == "Open" || staff.Count == 0 ? null : repId,
                        };
                        t.MarkAsCreated(Marker);
                        tickets.Add(t);
                    }
                    await db.SupportTickets.AddRangeAsync(tickets, ct);
                    await db.SaveChangesAsync(ct);
                }

                var demoCerts = new[]
                                {
                    ("ISO-13485-2024", "ISO 13485:2016 Quality Management", "BSI Group", 730),
                    ("CE-MDR-2024", "CE Mark — EU MDR 2017/745", "TÜV SÜD", 1095),
                    ("FDA-510K-2023", "FDA 510(k) Clearance", "U.S. Food & Drug Administration", 1825),
                    ("ISO-9001-2024", "ISO 9001:2015 Quality Management", "TÜV SÜD", 1095),
                    ("MDSAP-2024", "MDSAP Quality System Certificate", "BSI Group", 1095),
                    ("SFDA-2025", "Saudi FDA Medical Device Authorization", "Saudi Food & Drug Authority", 1095),
                };
                var existingCertNos = new HashSet<string>(
                    await db.Certifications.Where(c => !c.IsDeleted).Select(c => c.CertificateNumber).ToListAsync(ct),
                    StringComparer.OrdinalIgnoreCase);
                var certsToAdd = new List<Certification>();
                foreach (var (no, title, issuer, validDays) in demoCerts)
                {
                    if (existingCertNos.Contains(no)) continue;
                    certsToAdd.Add(Certification.Create(
                        no, title, "SNUL", issuer,
                        DateTime.UtcNow.AddDays(-180), DateTime.UtcNow.AddDays(validDays - 180),
                        $"Demo certification record: {title}.", $"demo/certs/{Slugify(no)}.jpg",
                        staff.Count > 0 ? staff[0].Id : null, Marker));
                }
                if (certsToAdd.Count > 0)
                {
                    await db.Certifications.AddRangeAsync(certsToAdd, ct);
                    await db.SaveChangesAsync(ct);
                }

                if (!await db.Carts.AnyAsync(c => !c.IsDeleted && c.CreatedBy == Marker, ct) && products.Count > 0)
                {
                    var cartOwners = activeMembers.Take(6).ToList();
                    if (cartOwners.Count == 0)
                        cartOwners = await db.ApplicationUsers.Where(u => !u.IsDeleted).Take(6).ToListAsync(ct);
                    var carts = new List<Cart>();
                    foreach (var owner in cartOwners)
                    {
                        var cart = new Cart { Id = Guid.NewGuid(), UserId = owner.Id, CurrencyId = usd?.Id };
                        cart.MarkAsCreated(Marker);
                        foreach (var p in PickSome(faker, products, 1, 3))
                        {
                            var ci = new CartItem
                            {
                                Id = Guid.NewGuid(),
                                CartId = cart.Id,
                                ProductId = p.Id,
                                Quantity = faker.Random.Int(1, 5),
                                UnitPriceSnapshot = p.Price,
                            };
                            ci.MarkAsCreated(Marker);
                            cart.Items.Add(ci);
                        }
                        carts.Add(cart);
                    }
                    for (var g = 0; g < 2; g++)
                    {
                        var guest = new Cart { Id = Guid.NewGuid(), SessionId = $"demo-session-{Guid.NewGuid():N}", CurrencyId = usd?.Id };
                        guest.MarkAsCreated(Marker);
                        foreach (var p in PickSome(faker, products, 1, 2))
                        {
                            var ci = new CartItem
                            {
                                Id = Guid.NewGuid(),
                                CartId = guest.Id,
                                ProductId = p.Id,
                                Quantity = faker.Random.Int(1, 4),
                                UnitPriceSnapshot = p.Price,
                            };
                            ci.MarkAsCreated(Marker);
                            guest.Items.Add(ci);
                        }
                        carts.Add(guest);
                    }
                    if (carts.Count > 0)
                    {
                        await db.Carts.AddRangeAsync(carts, ct);
                        await db.SaveChangesAsync(ct);
                    }
                }

                var interactionUsers = activeMembers.Take(10).ToList();
                if (interactionUsers.Count == 0)
                    interactionUsers = await db.ApplicationUsers.Where(u => !u.IsDeleted).Take(10).ToListAsync(ct);
                if (interactionUsers.Count > 0 && products.Count > 0
                    && !await db.UserProductInteractions.AnyAsync(i => !i.IsDeleted && i.CreatedBy == Marker, ct))
                {
                    var interactionTypes = new[] { "Wishlist", "RecentlyViewed", "Compare" };
                    var existingTriples = (await db.UserProductInteractions.Where(i => !i.IsDeleted)
                            .Select(i => new { i.UserId, i.ProductId, i.Type }).ToListAsync(ct))
                        .Select(x => $"{x.UserId}|{x.ProductId}|{x.Type}").ToHashSet();
                    var interactions = new List<UserProductInteraction>();
                    var attempts = 0;
                    while (interactions.Count < 30 && attempts++ < 300)
                    {
                        var u = faker.PickRandom(interactionUsers);
                        var p = faker.PickRandom(products);
                        var t = faker.PickRandom(interactionTypes);
                        if (!existingTriples.Add($"{u.Id}|{p.Id}|{t}")) continue;
                        interactions.Add(new UserProductInteraction
                        {
                            Id = Guid.NewGuid(),
                            UserId = u.Id,
                            ProductId = p.Id,
                            Type = t,
                            Timestamp = DateTime.UtcNow.AddDays(-faker.Random.Int(0, 30)),
                        });
                    }
                    foreach (var i in interactions) i.MarkAsCreated(Marker);
                    if (interactions.Count > 0)
                    {
                        await db.UserProductInteractions.AddRangeAsync(interactions, ct);
                        await db.SaveChangesAsync(ct);
                    }
                }

                if (!await db.DistributorApplications.AnyAsync(d => !d.IsDeleted, ct))
                {
                    var bands = new[] { "Under $100K", "$100K - $500K", "$500K - $1M", "$1M - $5M", "Over $5M" };
                    var appStatuses = new[]
                    {
                        DistributorApplicationStatus.Pending, DistributorApplicationStatus.Approved,
                        DistributorApplicationStatus.Pending, DistributorApplicationStatus.Rejected,
                        DistributorApplicationStatus.Approved,
                    };
                    var apps = new List<DistributorApplication>();
                    for (var i = 0; i < 5; i++)
                    {
                        var country = faker.PickRandom(countries);
                        var name = faker.Company.CompanyName();
                        var app = new DistributorApplication
                        {
                            Id = Guid.NewGuid(),
                            CompanyName = name,
                            CountryId = country.Id,
                            SalesVolumeBand = bands[i % bands.Length],
                            CategoryInterest = faker.PickRandom(Specialties).En,
                            Website = $"https://www.{Slugify(name)}.com",
                            ContactPerson = faker.Name.FullName(),
                            ContactEmail = faker.Internet.Email(),
                            Phone = faker.Phone.PhoneNumber("+9715########"),
                            Status = appStatuses[i],
                        };
                        app.MarkAsCreated(Marker);
                        apps.Add(app);
                    }
                    await db.DistributorApplications.AddRangeAsync(apps, ct);
                    await db.SaveChangesAsync(ct);
                }

                if (!await db.Documents.AnyAsync(d => !d.IsDeleted, ct))
                {
                    var docTypes = new[] { "Catalog", "Brochure", "IFU", "Certificate" };
                    var docs = new List<Document>();
                    for (var i = 0; i < 8; i++)
                    {
                        var dt = docTypes[i % docTypes.Length];
                        var linked = products.Count > 0 && i % 2 == 0 ? faker.PickRandom(products) : null;
                        var doc = new Document
                        {
                            Id = Guid.NewGuid(),
                            Title = linked != null ? $"{linked.NameEn} — {dt}" : $"SNUL {dt} {2024 + (i % 2)}",
                            DocType = dt,
                            FileUrl = $"demo/docs/{Slugify(dt)}-{i + 1:00}.pdf",
                            FileSizeKB = faker.Random.Int(200, 15000),
                            ProductId = linked?.Id,
                            PublishedDate = DateTime.UtcNow.AddDays(-faker.Random.Int(1, 180)),
                        };
                        doc.MarkAsCreated(Marker);
                        docs.Add(doc);
                    }
                    await db.Documents.AddRangeAsync(docs, ct);
                    await db.SaveChangesAsync(ct);
                }

                if (!await db.SupportContacts.AnyAsync(s => !s.IsDeleted, ct))
                {
                    var contact = new SupportContact
                    {
                        Id = Guid.NewGuid(),
                        SupportEmail = "support@snul.health",
                        PhoneNumber = "+971500000001",
                        WhatsAppNumber = "+971500000001",
                        WorkingHours = "Mon - Fri: 8:00 AM - 6:00 PM (GST)",
                    };
                    contact.MarkAsCreated(Marker);
                    await db.SupportContacts.AddAsync(contact, ct);
                    await db.SaveChangesAsync(ct);
                }

                if (!await db.LandingPages.AnyAsync(l => !l.IsDeleted && l.CreatedBy == Marker, ct))
                {
                    var existingSlugs = new HashSet<string>(
                        await db.LandingPages.Where(l => !l.IsDeleted).Select(l => l.Slug).ToListAsync(ct),
                        StringComparer.OrdinalIgnoreCase);
                    var extras = new[]
                    {
                        ("Specialty", "dental-instruments", "Dental instruments, manufacturer-direct",
                            "Forceps, elevators and mirrors crafted from German stainless steel."),
                        ("Procedure", "sterilization-guide", "Sterilization guide for reusable instruments",
                            "Autoclave at 134°C, DIN-grade materials, validated reprocessing steps."),
                    };
                    var pagesToAdd = new List<LandingPage>();
                    foreach (var (type, slug, hero, body) in extras)
                    {
                        if (existingSlugs.Contains(slug)) continue;
                        var page = new LandingPage
                        {
                            Id = Guid.NewGuid(),
                            Type = type,
                            Slug = slug,
                            HeroTitle = hero,
                            HeroBody = body,
                            ContentBlock = body,
                        };
                        page.MarkAsCreated(Marker);
                        pagesToAdd.Add(page);
                    }
                    if (pagesToAdd.Count > 0)
                    {
                        await db.LandingPages.AddRangeAsync(pagesToAdd, ct);
                        await db.SaveChangesAsync(ct);
                    }
                }

                var tokenUsers = activeMembers.Take(5).ToList();
                if (tokenUsers.Count == 0)
                    tokenUsers = await db.ApplicationUsers.Where(u => !u.IsDeleted).Take(5).ToListAsync(ct);
                if (tokenUsers.Count > 0 && !await db.UserRefreshTokens.AnyAsync(t => t.CreatedBy == Marker, ct))
                {
                    var tokens = tokenUsers.Select(u =>
                    {
                        var rt = UserRefreshToken.Create(u.Id, $"demo-{Guid.NewGuid():N}{Guid.NewGuid():N}", DateTime.UtcNow.AddDays(30));
                        rt.MarkAsCreated(Marker);
                        return rt;
                    }).ToList();
                    await db.UserRefreshTokens.AddRangeAsync(tokens, ct);
                    await db.SaveChangesAsync(ct);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<ApplicationUser?> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            Faker faker,
            string email,
            string fullName,
            UserType userType,
            Guid? companyId,
            AppLanguage language,
            string password,
            CancellationToken ct)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null) return existing;
            var user = new ApplicationUser
            {
                FullName = fullName,
                Email = email,
                UserName = email,
                PhoneNumber = faker.Phone.PhoneNumber("+9715########"),
                UserType = userType,
                CompanyId = companyId,
                Language = language,
                EmailConfirmed = true,
                IsActive = true,
            };
            user.MarkAsCreated(Marker);
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return null;
            }
            var roleResult = await userManager.AddToRoleAsync(user, userType.ToString());
            return user;
        }
    }
}
