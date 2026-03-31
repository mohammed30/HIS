import { Document, Packer, Paragraph, TextRun, ImageRun, HeadingLevel, AlignmentType, BorderStyle } from 'docx';
import fs from 'fs';
import path from 'path';

const imgDir = 'C:\\Users\\Mohammed\\.gemini\\antigravity\\brain\\137f8d98-2fb8-4f32-990f-b0325b341071';

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
        font: 'Cairo',
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
        font: 'Cairo',
        size: 24,
        rightToLeft: true,
      }),
    ],
  });
}

function numberedPara(num, text) {
  return rtlPara(`${num}. ${text}`);
}

function imagePara(filename, w = 600, h = 337) {
  const data = img(filename);
  if (!data) return rtlPara('[صورة توضيحية - غير متوفرة]');
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
      bottom: { style: BorderStyle.SINGLE, size: 6, color: '0078D4' },
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
            text: 'دليل المستخدم - نظام المختبر',
            font: 'Cairo',
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
            text: 'دورة عمل طلبات التحاليل الطبية والنتائج',
            font: 'Cairo',
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
            font: 'Cairo',
            size: 28,
            color: '666666',
            rightToLeft: true,
          }),
        ],
      }),

      separator(),

      // Section 1
      heading('1. استقبال المريض وطلب التحليل', HeadingLevel.HEADING_1),
      rtlPara('تبدأ الدورة من شاشة استقبال المختبر، حيث يتم تسجيل المريض أو اختياره واختيار الفحوصات المطلوبة.'),
      bulletPara('انتقل إلى قسم الاستقبال (Reception) ثم اختر استقبال المختبر (Lab Reception).'),
      bulletPara('أدخل بيانات المريض الجديد أو ابحث عن مريض سابق باستخدام رقم الملف أو الاسم.'),
      imagePara('lab_reception_main_1774773970574.png'),

      heading('أ. اختيار الفحوصات وإصدار الفاتورة', HeadingLevel.HEADING_2),
      numberedPara(1, 'من قائمة الفحوصات المتاحة، ابحث عن التحليل المطلوب واضغط لإضافته للطلب.'),
      numberedPara(2, 'تأكد من تسعير التحاليل بشكل صحيح حسب فئة المريض (نقدي/تأمين).'),
      numberedPara(3, 'اضغط على "حفظ الفاتورة" لإنهاء الطلب المالي وإرساله للمختبر.'),
      imagePara('lab_test_selection_final_1774774162405.png'),

      separator(),

      // Section 2
      heading('2. استلام الطلبات وسحب العينات', HeadingLevel.HEADING_1),
      rtlPara('تنتقل المهمة الآن لفني المختبر لمباشرة العمل الفني وسحب العينات.'),
      bulletPara('اذهب إلى قائمة المختبر (Laboratory) ثم طلبات المختبر (Lab Requests).'),
      bulletPara('ستظهر كافة الطلبات الواردة في قائمة الانتظار، مرتبة حسب الأولوية وتوقيت الطلب.'),
      imagePara('lab_requests_list_1774774127133.png'),

      heading('أ. سحب العينة وطباعة الباركود', HeadingLevel.HEADING_2),
      bulletPara('عند حضور المريض لسحب العينة، اضغط على زر "سحب العينة" (Collect Sample).'),
      bulletPara('سيقوم النظام آلياً بتوجيه أمر لطباعة ملصق الباركود الخاص بالعينة لضمان دقة التتبع.'),
      imagePara('sample_collection_barcode_1774774133920.png'),

      separator(),

      // Section 3
      heading('3. الرقابة المالية والقيود المحاسبية', HeadingLevel.HEADING_1),
      rtlPara('يقوم النظام بترحيل كافة الحركات المالية آلياً إلى قسم المحاسبة لضمان الرقابة التامة.'),
      bulletPara('بمجرد حفظ الفاتورة، يُنشئ النظام قيد استحقاق الإيراد.'),
      bulletPara('بمجرد سداد المبلغ، يُنشئ النظام قيد تحصيل يغلق مديونية المريض.'),
      bulletPara('يمكن استعراض القيود من قائمة "الحسابات" -> "قيود اليومية".'),
      imagePara('lab_journal_entries_1774774144393.png'),

      separator(),

      // Section 4
      heading('4. إدخال النتائج والاعتماد النهائي', HeadingLevel.HEADING_1),
      rtlPara('بعد الانتهاء من فحص العينة، يتم إدخال النتائج في النظام للمراجعة والطباعة.'),
      numberedPara(1, 'من شاشة طلبات المختبر، اختر "إدخال النتائج" للفحص المحدد.'),
      numberedPara(2, 'أدخل القيم المخبرية، وسينبهك النظام آلياً في حال كانت النتيجة خارج النطاق الطبيعي.'),
      numberedPara(3, 'بعد المراجعة، يضغط الأخصائي المسؤول على "اعتماد النتيجة" لتصبح رسمية.'),
      numberedPara(4, 'أخيراً، يمكن طباعة تقرير النتائج النهائي للمريض بضغطة زر.'),

      separator(),

      new Paragraph({
        alignment: AlignmentType.CENTER,
        spacing: { before: 400 },
        children: [
          new TextRun({
            text: 'نهاية الدليل - مستشفى آسيا',
            font: 'Cairo',
            size: 24,
            bold: true,
            color: '1F4E79',
            rightToLeft: true,
          }),
        ],
      }),
    ],
  }],
});

const outPath = path.join(imgDir, 'Laboratory_User_Guide.docx');
Packer.toBuffer(doc).then(buffer => {
  fs.writeFileSync(outPath, buffer);
  console.log('Laboratoy Word document created successfully at:', outPath);
});
