-- =============================================
-- SEED USERS
-- =============================================
--DELETE FROM Users
DELETE FROM Courses
DELETE FROM Lessons
DELETE FROM Enrollments
DELETE FROM UserLessonProgresses

INSERT INTO Users
(
    Email,
    PasswordHash,
    FullName,
    AvatarUrl,
    Bio,
    Role,
    IsVerified,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    'admin@overload.io',
    '$2a$11$dpDO4yuIv5Y/yDChDhf9cOl5J/0rsaCnsEX7HRaeGIKa6xZnl1M7K',
    'System Admin',
    'https://api.dicebear.com/7.x/initials/svg?seed=SA',
    'Platform administrator.',
    2,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    'john.instructor@overload.io',
    '$2a$11$U9tsNWJJMwn8gTOE.jjm4uUjl1LnvOTzC6hjqN4jf2YprbT3RP1Q6',
    'John Carter',
    'https://api.dicebear.com/7.x/initials/svg?seed=JC',
    'Senior software engineer with 10+ years of experience in .NET and cloud architecture.',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    'sarah.instructor@overload.io',
    '$2a$11$qCtb5UfFargcJaG/3J8WNepB7YUjHleCLOd8i50jII2gAB0Qxiu9i',
    'Sarah Mitchell',
    'https://api.dicebear.com/7.x/initials/svg?seed=SM',
    'Full-stack developer and React enthusiast.',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    'alice@student.com',
    '$2a$11$YjkhxW/KQLQncqP3mYPek.1qtpHkCxQ348XjrGhX8jYXKr/QM6xo.',
    'Alice Johnson',
    'https://api.dicebear.com/7.x/initials/svg?seed=AJ',
    'Aspiring backend developer.',
    0,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    'bob@student.com',
    '$2a$11$YTd2cpTTc36YqZhDd/l49uPjUM81aY5Z6Vdbzxfwg8f5z2vxC.jUS',
    'Bob Williams',
    'https://api.dicebear.com/7.x/initials/svg?seed=BW',
    'Computer science student interested in full-stack development.',
    0,
    1,
    GETUTCDATE(),
    GETUTCDATE()
);

-- =============================================
-- SEED COURSES
-- =============================================

INSERT INTO Courses
(
    Title,
    Slug,
    Description,
    ThumbnailUrl,
    Category,
    Level,
    IsPublished,
    TotalDurationMinutes,
    TotalLessons,
    CreatedAt,
    UpdatedAt,
    Price
)
VALUES
(
    'ASP.NET Core 9 Complete Guide',
    'aspnet-core-9-complete-guide',
    'Master ASP.NET Core 9 from scratch.',
    'https://placehold.co/600x400/3b82f6/ffffff?text=ASP.NET+Core+9',
    'Backend',
    1,
    1,
    233,
    8,
    GETUTCDATE(),
    GETUTCDATE(),
    100000
),
(
    'React 18 & TypeScript Masterclass',
    'react-18-typescript-masterclass',
    'Build modern React applications using TypeScript.',
    'https://placehold.co/600x400/06b6d4/ffffff?text=React+18',
    'Frontend',
    1,
    1,
    155,
    6,
    GETUTCDATE(),
    GETUTCDATE(),
    90000
),
(
    'SQL Server & EF Core Deep Dive',
    'sql-server-ef-core-deep-dive',
    'Deep dive into SQL Server and EF Core.',
    'https://placehold.co/600x400/f59e0b/ffffff?text=SQL+Server',
    'Database',
    2,
    1,
    163,
    5,
    GETUTCDATE(),
    GETUTCDATE(),
    80000
),
(
    'HTML Basic',
    'html-basic',
    'Learn the fundamentals of HTML including attributes, colors, and comments.',
    'https://placehold.co/600x400/f97316/ffffff?text=HTML+Basic',
    'Frontend',
    0, -- Beginner
    1,
    45,
    7,
    GETUTCDATE(),
    GETUTCDATE(),
    0
);

-- =============================================
-- SEED LESSONS
-- =============================================
DECLARE @HtmlCourseId INT;

SET @HtmlCourseId =
(
    SELECT TOP 1 Id
    FROM Courses
    WHERE Slug = 'html-basic'
);
DECLARE @AspNetCourseId INT =
(
    SELECT Id
    FROM Courses
    WHERE Slug = 'aspnet-core-9-complete-guide'
);

DECLARE @ReactCourseId INT =
(
    SELECT Id
    FROM Courses
    WHERE Slug = 'react-18-typescript-masterclass'
);

INSERT INTO Lessons
(
    CourseId,
    Title,
    Description,
    Content,
    DurationMinutes,
    OrderIndex,
    IsFree,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @AspNetCourseId,
    'Introduction to ASP.NET Core 9',
    'Overview of the framework.',
    '# Introduction',
    12,
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    @AspNetCourseId,
    'Setting Up Your Development Environment',
    'Install .NET 9 SDK.',
    '# Setup',
    18,
    2,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    @AspNetCourseId,
    'Understanding Middleware Pipeline',
    'Learn request flow.',
    '# Middleware',
    25,
    3,
    0,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    @ReactCourseId,
    'React 18 & TypeScript Setup',
    'Vite and project structure.',
    '# React',
    15,
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    @ReactCourseId,
    'Components, Props & State',
    'Functional components.',
    '# Components',
    22,
    2,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
--lesson 1
    @HtmlCourseId,
    'HTML Attributes',
    'Learn how HTML attributes provide additional information for elements.',
    '
<div style="font-family: Arial, sans-serif; line-height: 1.8;">

<h1>HTML Attributes</h1>

<p>
All HTML elements can have attributes.
Attributes provide additional information about elements.
</p>

<h2>The href Attribute</h2>

<pre>
&lt;a href="https://www.w3schools.com"&gt;
Visit W3Schools
&lt;/a&gt;
</pre>

<h2>The src Attribute</h2>

<pre>
&lt;img src="img_girl.jpg"&gt;
</pre>

<h2>The width and height Attributes</h2>

<pre>
&lt;img src="img_girl.jpg" width="500" height="600"&gt;
</pre>

<h2>The alt Attribute</h2>

<pre>
&lt;img src="img_girl.jpg" alt="Girl with a jacket"&gt;
</pre>

<h2>The style Attribute</h2>

<pre>
&lt;p style="color:red;"&gt;
This is a red paragraph.
&lt;/p&gt;
</pre>

<h2>The lang Attribute</h2>

<pre>
&lt;html lang="en"&gt;
</pre>

<h2>The title Attribute</h2>

<pre>
&lt;p title="I am tooltip"&gt;
This is a paragraph.
&lt;/p&gt;
</pre>

</div>
',
    15,
    4,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),

-- Lesson 2
(
    @HtmlCourseId,
    'HTML Colors',
    'Learn how to use colors in HTML with names, HEX, RGB, and HSL.',
    '
<div style="font-family: Arial, sans-serif; line-height: 1.8;">

<h1>HTML Colors</h1>

<p>
HTML supports 140 standard color names.
</p>

<h2>Background Color</h2>

<pre>
&lt;h1 style="background-color:DodgerBlue;"&gt;
Hello World
&lt;/h1&gt;
</pre>

<h2>Text Color</h2>

<pre>
&lt;p style="color:DodgerBlue;"&gt;
Lorem ipsum...
&lt;/p&gt;
</pre>

<h2>Border Color</h2>

<pre>
&lt;h1 style="border:2px solid Tomato;"&gt;
Hello World
&lt;/h1&gt;
</pre>

<h2>Color Values</h2>

<ul>
<li>RGB</li>
<li>HEX</li>
<li>HSL</li>
<li>RGBA</li>
<li>HSLA</li>
</ul>

<h2>RGB Example</h2>

<pre>
rgb(255, 99, 71)
</pre>

<h2>HEX Example</h2>

<pre>
#ff6347
</pre>

<h2>HSL Example</h2>

<pre>
hsl(9, 100%, 64%)
</pre>

</div>
',
    15,
    2,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
-- Lesson 3
(
    @HtmlCourseId,
    'HTML Comment Tag',
    'Learn how to write comments in HTML and use them for debugging.',
    '
<div style="font-family: Arial, sans-serif; line-height: 1.8;">

<h1>HTML Comment Tag</h1>

<h2>Comment Syntax</h2>

<pre>
&lt;!-- Write your comments here --&gt;
</pre>

<p>
Comments are not displayed in the browser.
</p>

<h2>Add Comments</h2>

<pre>
&lt;!-- This is a comment --&gt;

&lt;p&gt;This is a paragraph.&lt;/p&gt;
</pre>

<h2>Hide Content with Comments</h2>

<pre>
&lt;!-- &lt;p&gt;This paragraph is hidden&lt;/p&gt; --&gt;
</pre>

<h2>Hide Multiple Lines</h2>

<pre>
&lt;!--
&lt;p&gt;Hidden paragraph&lt;/p&gt;
&lt;img src="pic.jpg"&gt;
--&gt;
</pre>

<h2>Debugging with Comments</h2>

<pre>
&lt;!-- Temporarily disable this image --&gt;
&lt;!-- &lt;img src="logo.png"&gt; --&gt;
</pre>

</div>
',
    15,
    3,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
--lesson 4
( @HtmlCourseId,
'Introduction to HTML',
'Introduction to HTML',
'<h1>Introduction to HTML</h1>

<p>
    HTML stands for <strong>Hyper Text Markup Language</strong>.
</p>

<p>
    HTML is the standard markup language for creating web pages.
</p>

<h2>What is HTML?</h2>

<ul>
    <li>HTML describes the structure of a web page.</li>
    <li>HTML consists of a series of elements.</li>
    <li>HTML elements tell the browser how to display the content.</li>
    <li>
        HTML elements label pieces of content such as:
        <ul>
            <li>this is a heading</li>
            <li>this is a paragraph</li>
            <li>this is a link</li>
        </ul>
    </li>
</ul>

<h2>What is an HTML Element?</h2>

<p>
    An HTML element is defined by a start tag, some content, and an end tag:
</p>

<pre>
&lt;tagname&gt; Content goes here... &lt;/tagname&gt;
</pre>

<p>
    The HTML element is everything from the start tag to the end tag:
</p>

<pre>
&lt;h1&gt;My First Heading&lt;/h1&gt;
&lt;p&gt;My first paragraph.&lt;/p&gt;
</pre>

<h3>Example Output</h3>

<h1>My First Heading</h1>
<p>My first paragraph.</p>',15,1,1,GETUTCDATE(),GETUTCDATE()),(@HtmlCourseId,
'HTML Elements','HTML Elements',
'<h1>HTML Elements</h1>

<p>
    The HTML element is everything from the start tag to the end tag:
</p>

<pre>
&lt;tagname&gt;Content goes here...&lt;/tagname&gt;
</pre>

<h2>Examples of HTML Elements</h2>

<p>Examples of some HTML elements:</p>

<pre>
&lt;h1&gt;My First Heading&lt;/h1&gt;
&lt;p&gt;My first paragraph.&lt;/p&gt;
</pre>

<h3>Rendered Example</h3>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

<hr />

<h1>Nested HTML Elements</h1>

<p>
    HTML elements can be nested (this means that elements can contain other elements).
</p>

<p>
    All HTML documents consist of nested HTML elements.
</p>

<p>
    The following example contains four HTML elements:
    <code>&lt;html&gt;</code>,
    <code>&lt;body&gt;</code>,
    <code>&lt;h1&gt;</code>
    and
    <code>&lt;p&gt;</code>.
</p>

<h2>Nested Elements Example</h2>

<pre>
&lt;html&gt;
    &lt;body&gt;

        &lt;h1&gt;My First Heading&lt;/h1&gt;
        &lt;p&gt;My first paragraph.&lt;/p&gt;

    &lt;/body&gt;
&lt;/html&gt;
</pre>',15,5,1,GETUTCDATE(),GETUTCDATE()),(@HtmlCourseId,
N'HTML Interactive Box',
N'Thực hành thay đổi màu sắc, bo tròn góc và đổ bóng cho một khối div box bằng CSS trực quan.',
N'<div style="font-family: Arial, sans-serif; line-height: 1.8;">

<h1>HTML Interactive Box</h1>
<p>Thẻ div là một khối chứa (container block) dùng để gom nhóm các phần tử và áp dụng định dạng CSS.</p>

<h2>Tạo một Box cơ bản</h2>
<p>Đầu tiên, hãy tạo một chiếc hộp với border và kích thước xác định.</p>
<pre>
&lt;div style="width: 200px; height: 200px; border: 2px dashed #6366f1; transition: all 0.5s ease;"&gt;&lt;/div&gt;
</pre>

<h2>Thêm màu nền cho Box</h2>
<p>Bây giờ, hãy đổi màu nền của chiếc hộp thành màu Indigo (Xanh tím thuốc nhuộm) sử dụng thuộc tính background-color.</p>
<pre>
&lt;div style="width: 200px; height: 200px; border: 2px solid #6366f1; background-color: #6366f1; transition: all 0.5s ease;"&gt;&lt;/div&gt;
</pre>

<h2>Bo tròn các góc Box</h2>
<p>Chúng ta có thể dùng thuộc tính border-radius để bo tròn các góc, biến chiếc hộp vuông thành một hình tròn.</p>
<pre>
&lt;div style="width: 200px; height: 200px; border: 2px solid #6366f1; background-color: #6366f1; border-radius: 50%; transition: all 0.5s ease;"&gt;&lt;/div&gt;
</pre>

<h2>Thêm hiệu ứng đổ bóng</h2>
<p>Cuối cùng, hãy thêm box-shadow để chiếc hộp trông nổi bật và 3D hơn.</p>
<pre>
&lt;div style="width: 200px; height: 200px; border: 2px solid #6366f1; background-color: #6366f1; border-radius: 50%; box-shadow: 0 10px 25px rgba(99, 102, 241, 0.4); transition: all 0.5s ease;"&gt;&lt;/div&gt;
</pre>

</div>',15,6,1,GETUTCDATE(),GETUTCDATE()),
(@HtmlCourseId,
N'HTML Interactive Counter',
N'Học cách thiết kế bộ đếm lượt click tương tác bằng HTML, CSS, và JavaScript thực tế.',
N'<div style="font-family: Arial, sans-serif; line-height: 1.8;">

<h1>Xây dựng nút bấm tương tác (Interactive Click Counter)</h1>
<p>Một website hiện đại không chỉ có cấu trúc HTML và phong cách CSS, mà còn cần JavaScript để phản hồi các hành động của người dùng.</p>

<h2>Bước 1: Thiết kế giao diện cơ bản (HTML)</h2>
<p>Chúng ta sẽ tạo một container chứa một tiêu đề, một nút bấm &lt;code&gt;&amp;lt;button&amp;gt;&lt;/code&gt; và một thẻ hiển thị số lượt click.</p>
<pre>
&lt;div class="card"&gt;
  &lt;h3&gt;Bộ Đếm Lượt Click&lt;/h3&gt;
  &lt;button id="click-btn"&gt;Click Me! ⚡&lt;/button&gt;
  &lt;div id="counter"&gt;Lượt click: 0&lt;/div&gt;
&lt;/div&gt;
</pre>

<h2>Bước 2: Định hình kiểu dáng Premium (CSS)</h2>
<p>Sử dụng CSS để biến nút bấm thô sơ thành một nút bấm mang phong cách Modern Glassmorphism, bo tròn mềm mại và chuyển động mượt mà khi hover.</p>
<pre>
&lt;style&gt;
  .card {
    background: #ffffff;
    padding: 2.5rem;
    border-radius: 20px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
    text-align: center;
    border: 1px solid #e2e8f0;
    max-width: 320px;
    margin: 20px auto;
  }
  h3 { color: #1e293b; margin-bottom: 1.5rem; }
  button {
    background: linear-gradient(135deg, #3b82f6, #8b5cf6);
    color: white;
    font-size: 1rem;
    font-weight: 600;
    padding: 0.8rem 1.8rem;
    border: none;
    border-radius: 12px;
    cursor: pointer;
    box-shadow: 0 4px 15px rgba(59, 130, 246, 0.3);
    transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  }
  button:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(59, 130, 246, 0.4);
  }
  button:active {
    transform: translateY(1px) scale(0.97);
  }
  #counter {
    margin-top: 1.5rem;
    font-size: 1.25rem;
    font-weight: 700;
    color: #4f46e5;
  }
&lt;/style&gt;

&lt;div class="card"&gt;
  &lt;h3&gt;Bộ Đếm Lượt Click&lt;/h3&gt;
  &lt;button id="click-btn"&gt;Click Me! ⚡&lt;/button&gt;
  &lt;div id="counter"&gt;Lượt click: 0&lt;/div&gt;
&lt;/div&gt;
</pre>

<h2>Bước 3: Lập trình Logic tương tác (JavaScript)</h2>
<p>Bây giờ, chúng ta sẽ thêm mã JavaScript để lắng nghe sự kiện &lt;code&gt;click&lt;/code&gt; của nút bấm, tự động tăng số lượng và cập nhật trực tiếp nội dung giao diện.</p>
<pre>
&lt;style&gt;
  .card {
    background: #ffffff;
    padding: 2.5rem;
    border-radius: 20px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
    text-align: center;
    border: 1px solid #e2e8f0;
    max-width: 320px;
    margin: 20px auto;
  }
  h3 { color: #1e293b; margin-bottom: 1.5rem; }
  button {
    background: linear-gradient(135deg, #3b82f6, #8b5cf6);
    color: white;
    font-size: 1rem;
    font-weight: 600;
    padding: 0.8rem 1.8rem;
    border: none;
    border-radius: 12px;
    cursor: pointer;
    box-shadow: 0 4px 15px rgba(59, 130, 246, 0.3);
    transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  }
  button:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(59, 130, 246, 0.4);
  }
  button:active {
    transform: translateY(1px) scale(0.97);
  }
  #counter {
    margin-top: 1.5rem;
    font-size: 1.25rem;
    font-weight: 700;
    color: #4f46e5;
  }
&lt;/style&gt;

&lt;div class="card"&gt;
  &lt;h3&gt;Bộ Đếm Lượt Click&lt;/h3&gt;
  &lt;button id="click-btn"&gt;Click Me! ⚡&lt;/button&gt;
  &lt;div id="counter"&gt;Lượt click: 0&lt;/div&gt;
&lt;/div>

&lt;script&gt;
  const btn = document.getElementById(''click-btn'');
  const display = document.getElementById(''counter'');
  let count = 0;

  btn.addEventListener(''click'', () => {
    count++;
    display.textContent = `Lượt click: \${count}`;
  });
&lt;/script&gt;
</pre>

<h2>Bước 4: Nâng cao hiệu ứng bằng JavaScript</h2>
<p>Cuối cùng, chúng ta sẽ viết thêm hiệu ứng rung lắc nhỏ hoặc đổi màu ngẫu nhiên khi nút được bấm thành công bằng cách cập nhật động style CSS trong JavaScript!</p>
<pre>
&lt;style&gt;
  .card {
    background: #ffffff;
    padding: 2.5rem;
    border-radius: 20px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
    text-align: center;
    border: 1px solid #e2e8f0;
    max-width: 320px;
    margin: 20px auto;
  }
  h3 { color: #1e293b; margin-bottom: 1.5rem; }
  button {
    background: linear-gradient(135deg, #3b82f6, #8b5cf6);
    color: white;
    font-size: 1rem;
    font-weight: 600;
    padding: 0.8rem 1.8rem;
    border: none;
    border-radius: 12px;
    cursor: pointer;
    box-shadow: 0 4px 15px rgba(59, 130, 246, 0.3);
    transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  }
  button:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(59, 130, 246, 0.4);
  }
  button:active {
    transform: translateY(1px) scale(0.97);
  }
  #counter {
    margin-top: 1.5rem;
    font-size: 1.25rem;
    font-weight: 700;
    color: #4f46e5;
    transition: transform 0.1s ease;
  }
&lt;/style&gt;

&lt;div class="card"&gt;
  &lt;h3&gt;Bộ Đếm Lượt Click&lt;/h3&gt;
  &lt;button id="click-btn"&gt;Click Me! ⚡&lt;/button&gt;
  &lt;div id="counter"&gt;Lượt click: 0&lt;/div&gt;
&lt;/div&gt;

&lt;script&gt;
  const btn = document.getElementById(''click-btn'');
  const display = document.getElementById(''counter'');
  let count = 0;

  btn.addEventListener(''click'', () => {
    count++;
    display.textContent = `Lượt click: \${count}`;
    
    display.style.transform = ''scale(1.2)'';
    setTimeout(() => {
      display.style.transform = ''scale(1)'';
    }, 100);
  });
&lt;/script&gt;
</pre>

</div>',15,7,1,GETUTCDATE(),GETUTCDATE());
