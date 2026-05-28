DECLARE @HtmlCourseId INT;
SET @HtmlCourseId = (SELECT TOP 1 Id FROM Courses WHERE Slug = 'html-basic');

-- Delete if already exists to prevent duplicate key
DELETE FROM Lessons WHERE CourseId = @HtmlCourseId AND Title = N'HTML Interactive Counter';

INSERT INTO Lessons (CourseId, Title, Description, Content, DurationMinutes, OrderIndex, IsFree, CreatedAt, UpdatedAt)
VALUES (
    @HtmlCourseId,
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
&lt;/div&gt;

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

</div>',
    15,
    7,
    1,
    GETUTCDATE(),
    GETUTCDATE()
);

-- Sync TotalLessons for course
UPDATE Courses 
SET TotalLessons = (SELECT COUNT(*) FROM Lessons WHERE CourseId = @HtmlCourseId) 
WHERE Id = @HtmlCourseId;
