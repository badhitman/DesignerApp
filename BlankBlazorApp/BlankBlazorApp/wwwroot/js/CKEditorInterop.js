window.CKEditorInterop = (() => {
    const editors = {};

    return {
        init(id, isReadOnly, dotNetReference) {
            CKSource.Editor
                .create(document.getElementById(id))
                .then(editor => {
                    if (isReadOnly == true) {
                        editor.isReadOnly = true;
                    } else {
                        editors[id] = editor;
                        editor.model.document.on('change:data', () => {
                            let data = editor.getData();

                            const el = document.createElement('div');
                            el.innerHTML = data;
                            if (el.innerText.trim() == '')
                                data = null;

                            dotNetReference.invokeMethodAsync('EditorDataChanged', data);
                        });
                    }
                })
                .catch(error => console.error(error));
        },
        destroy(id) {
            editors[id].destroy()
                .then(() => delete editors[id])
                .catch(error => console.log(error));
        }
    };
})();