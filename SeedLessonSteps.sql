-- Update the existing lesson with interactive steps JSON content
UPDATE lessons 
SET content = N'{
  "total_steps": 3,
  "steps": [
    {
      "step_index": 1,
      "trigger_percentage": 15.00,
      "narrative": "Bước 1: Chúng ta sẽ tạo một khung container cơ bản bằng thẻ HTML div.",
      "code_action": "insert",
      "code_snippet": "<div class=''box''>\n  <!-- Content here -->\n</div>",
      "ui_render_state": {
        "component": "CodeBoxContainer",
        "props": { "borderColor": "gray", "dimensions": "200x200" }
      },
      "checkpoint": null
    },
    {
      "step_index": 2,
      "trigger_percentage": 45.00,
      "narrative": "Bước 2: Thêm CSS để biến cái hộp thành màu xanh và bo tròn góc.",
      "code_action": "append",
      "code_snippet": ".box {\n  background: #4CAF50;\n  border-radius: 8px;\n}",
      "ui_render_state": {
        "component": "CodeBoxContainer",
        "props": { "backgroundColor": "#4CAF50", "borderRadius": "8px" }
      },
      "checkpoint": null
    },
    {
      "step_index": 3,
      "trigger_percentage": 75.00,
      "narrative": "Bước 3: Hãy kiểm tra kiến thức! Trả lời câu hỏi bên dưới.",
      "code_action": "none",
      "code_snippet": "",
      "ui_render_state": null,
      "checkpoint": {
        "checkpoint_index": 1,
        "type": "multiple_choice",
        "question": "Thuộc tính CSS nào dùng để bo tròn góc?",
        "options": ["border-radius", "corner-radius", "border-round", "round-corner"],
        "correct_answer": "border-radius"
      }
    }
  ]
}'
WHERE id = '7C46C97F-C190-4822-B1E5-3A7F135B293D';
GO
