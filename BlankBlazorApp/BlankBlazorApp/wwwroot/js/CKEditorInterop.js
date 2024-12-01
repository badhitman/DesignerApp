window.CKEditorInterop = (() => {
    const editors = {};

    return {
        init(id, isReadOnly, dotNetReference) {
            ClassicEditor
                .create(document.getElementById(id))
                .then(editor => {
                    editors[id] = editor;
                    if (isReadOnly == true) {
                        editor.enableReadOnlyMode(id);
                    }
                    editor.model.document.on('change:data', () => {
                        let data = editor.getData();
                        console.debug(`editor.model.document.on 'change:data': ${data}`);
                        const el = document.createElement('div');

                        // let pd = $(el).closest('#issue-messages-table-wrap').attr('id2','id3');
                       
                        el.innerHTML = data;
                        if (el.innerText.trim() == '')
                            data = null;

                        dotNetReference.invokeMethodAsync('EditorDataChanged', data);
                    });
                })
                .catch(error => console.error(error));
        },
        isReadOnly(id, is_read_only) {
            if (editors.hasOwnProperty(id)) {
                if (is_read_only == true) {
                    editors[id].enableReadOnlyMode(id);
                }
                else {
                    editors[id].disableReadOnlyMode(id);
                }
            }
        },
        setValue(id, val) {
            if (val == null)
                val = "";

            if (editors.hasOwnProperty(id)) {
                editors[id].data.set(val);
            }
        },
        destroy(id) {
            editors[id].destroy()
                .then(() => delete editors[id])
                .catch(error => console.log(error));
        }
    };
})();