import { Document, Packer, Paragraph, TextRun, ImageRun, HeadingLevel, AlignmentType, BorderStyle, Table, TableRow, TableCell, WidthType } from 'docx';
import fs from 'fs';
import path from 'path';

const imgDir = 'C:\\Users\\Mohammed\\.gemini\\antigravity\\brain\\d5f77ad9-c56c-4963-bc53-791098a0a05c';

function img(filename) {
  const p = path.join(imgDir, filename);
  if (!fs.existsSync(p)) {
    console.warn('Image not found:', p);
    return null;
  }
  return fs.readFileSync(p);
}

function heading(text, level = HeadingLevel.HEADING_1) {
  return new Paragraph({
    text,
    heading: level,
    alignment: AlignmentType.RIGHT,
    spacing: { before: 400, after: 200 },
    bidirectional: true,
  });
}

function rtlPara(text, opts = {}) {
  return new Paragraph({
    alignment: AlignmentType.RIGHT,
    bidirectional: true,
    spacing: { after: 120 },
    ...opts,
    children: [
      new TextRun({
        text,
        font: 'Calibri',
        size: 24,
        rightToLeft: true,
        ...opts.run,
      }),
    ],
  });
}

function bulletPara(text) {
  return new Paragraph({
    alignment: AlignmentType.RIGHT,
    bidirectional: true,
    spacing: { after: 80 },
    bullet: { level: 0 },
    children: [
      new TextRun({
        text,
        font: 'Calibri',
        size: 24,
        rightToLeft: true,
      }),
    ],
  });
}

function numberedPara(num, text) {
  return rtlPara(`${num}. ${text}`);
}

function imagePara(filename, w = 600, h = 350) {
  const data = img(filename);
  if (!data) return rtlPara('[صورة غير متوفرة]');
  return new Paragraph({
    alignment: AlignmentType.CENTER,
    spacing: { before: 200, after: 300 },
    children: [
      new ImageRun({
        data,
        transformation: { width: w, height: h },
        type: 'png',
      }),
    ],
  });
}

function separator() {
  return new Paragraph({
    spacing: { before: 200, after: 200 },
    border: {
      bottom: { style: BorderStyle.SINGLE, size: 6, color: '3366CC' },
    },
  });
}

const doc = new Document({
  sections: [{
    properties: {
      page: {
        margin: { top: 720, bottom: 720, left: 720, right: 720 },
      },
    },
    children: [
      // Title
      new Paragraph({
        alignment: AlignmentType.CENTER,
        spacing: { after: 400 },
        children: [
          new TextRun({
            text: 'دليل المستخدم',
            font: 'Calibri',
            size: 52,
            bold: true,
            color: '1F4E79',
            rightToLeft: true,
          }),
        ],
      }),
      new Paragraph({
        alignment: AlignmentType.CENTER,
        spacing: { after: 200 },
        children: [
          new TextRun({
            text: 'إدارة التنويم والخدمات الطبية',
            font: 'Calibri',
            size: 36,
            bold: true,
            color: '2E75B6',
            rightToLeft: true,
          }),
        ],
      }),
      new Paragraph({
        alignment: AlignmentType.CENTER,
        spacing: { after: 600 },
        children: [
          new TextRun({
            text: 'نظام إدارة المستشفيات (HIS) - مستشفى آسيا',
            font: 'Calibri',
            size: 28,
            color: '666666',
            rightToLeft: true,
          }),
        ],
      }),

      separator(),

      // Section 1
      heading('1. الدخول إلى شاشة إدارة التنويم', HeadingLevel.HEADING_1),
      bulletPara('من القائمة الرئيسية، اذهب إلى قسم التنويم (Inpatient) ثم اختر إدارة التنويم (Admissions).'),
      bulletPara('ستظهر لك قائمة بكافة المرضى المنومين حالياً وحالاتهم، بالإضافة لفلاتر البحث.'),
      imagePara('inpatient_management_1774748324518.png'),

      separator(),

      // Section 2
      heading('2. تسجيل تنويم جديد (تسكين المريض)', HeadingLevel.HEADING_1),
      rtlPara('لتسكين مريض جديد في سرير، اتبع الخطوات التالية:', { run: { bold: true } }),
      numberedPara(1, 'انقر على زر "تنويم جديد" أعلى الشاشة.'),
      numberedPara(2, 'ستظهر نافذة "بيانات التنويم".'),
      numberedPara(3, 'ابحث عن المريض بالاسم أو الرقم الطبي (MRN) واختره من القائمة.'),
      numberedPara(4, 'حدد نوع الغرفة (مثلاً: جناح، عناية مركزة).'),
      numberedPara(5, 'بناءً على نوع الغرفة، حدد الغرفة المطلوبة، ثم اختر السرير المتاح.'),
      numberedPara(6, 'املأ بيانات المرافق (إن وجدت) وملاحظات الدخول، ثم اضغط حفظ.'),
      imagePara('new_admission_modal_1774748332835.png'),

      separator(),

      // Section 3
      heading('3. طلب الخدمات والمستهلكات', HeadingLevel.HEADING_1),
      rtlPara('بمجرد تسكين المريض، يمكنك تحديد سجله من القائمة الرئيسية لتتفاعل معه.', { run: { bold: true } }),

      heading('أ. طلب مستهلكات طبية', HeadingLevel.HEADING_2),
      numberedPara(1, 'بعد تحديد المريض، انقر على خيار "طلب مستهلكات" من الأزرار السفلية.'),
      numberedPara(2, 'ابحث عن الصنف المطلوب إضافته للمريض وحدد الكمية، ثم اضغط حفظ.'),
      imagePara('order_consumables_modal_1774748376498.png'),

      heading('ب. طلب خدمات طبية (تحاليل وأشعة) وإجراء عمليات', HeadingLevel.HEADING_2),
      bulletPara('يمكنك بنفس الطريقة السابقة النقر على "طلب تحاليل" أو "طلب أشعة" لاختيار الخدمة.'),
      bulletPara('يمكنك إضافة تكاليف وأجور العمليات من خلال طلب خدمة طبية، حيث تُسجل تكلفتها فوراً على حساب المريض.'),

      separator(),

      // Section 4
      heading('4. الإدارة المالية وحساب المريض', HeadingLevel.HEADING_1),
      rtlPara('يتم تسجيل جميع الحركات المالية آلياً لتتمكن من مراجعتها في أي وقت.', { run: { bold: true } }),

      heading('أ. دفع مبالغ تحت الحساب (إيداع)', HeadingLevel.HEADING_2),
      numberedPara(1, 'حدد المريض واضغط على "مبلغ تحت الحساب".'),
      numberedPara(2, 'أدخل المبلغ المراد دفعه، وطريقة الدفع (نقدي/بطاقة)، ثم احفظ.'),
      imagePara('deposit_payment_modal_1774748418628.png'),

      heading('ب. استعراض الفاتورة المبدئية وحساب المريض', HeadingLevel.HEADING_2),
      numberedPara(1, 'انقر على "فاتورة مبدئية" لمراجعة تكلفة إقامة المريض والمستهلكات.'),
      numberedPara(2, 'سيعرض التقرير إجمالي الحركات والمبلغ المستحق.'),
      imagePara('patient_statement_1774748454400.png'),

      separator(),

      // Section 5
      heading('5. إجراء الخروج الطبي والمالي (Discharge)', HeadingLevel.HEADING_1),
      numberedPara(1, 'حدد المريض من واجهة التنويم الرئيسية، واضغط "خروج المريض".'),
      numberedPara(2, 'تحقق من تاريخ الخروج وضع أي ملاحظات ختامية.'),
      numberedPara(3, 'بمجرد تأكيد الخروج، سيقوم النظام تلقائياً بإنشاء الفاتورة النهائية وتفريغ السرير.'),
      imagePara('discharge_modal_1774748433533.png'),

      separator(),

      // Section 6
      heading('6. القيود المحاسبية (قيود اليومية)', HeadingLevel.HEADING_1),
      bulletPara('عند كل حركة مالية (فاتورة، صرف مستهلك، دفع تحت الحساب) يتم إنشاء قيد محاسبي مزدوج تلقائياً.'),
      bulletPara('للاطلاع عليها، اذهب إلى قائمة "الحسابات" واختر "قيود اليومية".'),
      bulletPara('ستظهر لك قائمة بجميع الحركات المُرحلة بالمبالغ المدينة والدائنة.'),
      imagePara('journal_entries_1774748475194.png'),
    ],
  }],
});

const outPath = path.join(imgDir, 'Inpatient_User_Guide.docx');
Packer.toBuffer(doc).then(buffer => {
  fs.writeFileSync(outPath, buffer);
  console.log('Word document created successfully at:', outPath);
});
